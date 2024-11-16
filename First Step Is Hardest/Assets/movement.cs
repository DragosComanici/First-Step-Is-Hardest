using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 200f;
    public float sprintSpeed = 10f;
    private float currentSpeed;

    public float grappleForce = 10f; // Force applied during grapple
    public float maxGrappleDistance = 20f; // Max distance for grapple
    public float grappleCooldown = 2f; // Cooldown time in seconds

    public float vaultDistance = 1f; // Max distance to detect an edge for vaulting
    public float vaultHeight = 1.5f; // The height to move the player onto the platform
    public float vaultSpeed = 5f; // Speed of vaulting motion

    private bool isGrounded = true;
    private bool canDoubleJump = false; // To track if the player can double jump
    private bool isJumping = false; // To track if the player has already jumped
    private bool hasDoubleJumped = false; // To track if the player has used double jump

    private Rigidbody rb;

    // Crouching variables
    private Vector3 originalScale;
    private Vector3 crouchedScale; // Height reduced but same width and depth
    private BoxCollider playerCollider; // Reference to the player's collider
    public float crouchedColliderHeight = 2f; // Height of collider when crouching
    public float originalColliderHeight; // Original height of the collider

    // Noclip variables
    private bool isNoclip = false; // Track noclip state
    public float noclipSpeed = 10f; // Speed during noclip
    private Collider playerColliderComponent; // Reference to the player's collider component

    // Double jump variables
    public bool doubleJumpUnlocked = false; // Whether double jump is unlocked

    // Grapple lock mechanic variables
    public bool grappleLockUnlocked = false; // Whether grapple lock is unlocked
    private bool isLockedToObject = false; // Whether the player is locked to an object during grapple lock
    private Transform lockedObject; // The object the player is locked to

    void Start()
    {
        currentSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody
        playerCollider = GetComponent<BoxCollider>(); // Get the BoxCollider component
        originalScale = transform.localScale; // Store original scale
        originalColliderHeight = playerCollider.size.y; // Store original collider height
        playerColliderComponent = GetComponent<Collider>(); // Get the Collider component

        crouchedScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z); // Half the height
    }

    void Update()
    {
        // Noclip toggle
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleNoclip();
        }

        if (isNoclip)
        {
            NoclipMovement(); // Handle noclip movement if active
        }
        else
        {
            // Crouching
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Crouch();
            }
            else
            {
                StandUp();
            }

            // Sprinting - make sure to set speed before movement
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = moveSpeed;
            }

            // Movement
            float moveX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
            float moveZ = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;

            // Combine horizontal movement and apply it to the rigidbody
            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            // Jumping (including double jump logic)
            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (doubleJumpUnlocked && !hasDoubleJumped)))
            {
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Jump force in the upward direction
                    isGrounded = false; // Set to false while in the air
                    isJumping = true; // The player has jumped
                    hasDoubleJumped = false; // Reset double jump state
                }
                else if (doubleJumpUnlocked && !isGrounded && !hasDoubleJumped)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Double jump force
                    hasDoubleJumped = true; // Disable further double jumps
                }
            }

            // Grapple mechanic with cooldown check (only if grapple lock is unlocked)
            if (grappleLockUnlocked && Input.GetMouseButtonDown(0) && Time.time >= grappleCooldown)
            {
                GrappleLock();
            }

            // Vaulting mechanic
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded) // Only vault if not grounded
            {
                AttemptVault();
            }

            // Apply movement to Rigidbody (while also considering the current speed)
            rb.MovePosition(rb.position + move); // Use MovePosition to avoid physics conflicts
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isNoclip && other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Reset to grounded when touching the ground
            isJumping = false; // Allow jumping again
            hasDoubleJumped = false; // Reset double jump state
        }
    }

    void GrappleLock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGrappleDistance))
        {
            if (hit.collider != null)
            {
                Vector3 grapplePoint = hit.point; // Point where the ray hit
                lockedObject = hit.collider.transform; // Lock to the object
                isLockedToObject = true; // Enable locking

                // Optional: Do something to visually indicate the player is locked, like changing the UI or player state
            }
        }
    }

    void ApplyGrappleForce(Vector3 targetPoint)
    {
        if (isLockedToObject)
        {
            // Calculate the direction to move around the object (lock movement to spherical orbit)
            Vector3 direction = (targetPoint - transform.position).normalized;
            rb.velocity = Vector3.zero; // Stop current movement
            rb.AddForce(direction * grappleForce, ForceMode.Impulse); // Apply force to move around the locked object
        }
    }

    private void AttemptVault()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 rayOrigin = transform.position + Vector3.up * (originalColliderHeight / 2); // Start ray a bit above player's feet

        if (Physics.Raycast(rayOrigin, forward, out hit, vaultDistance))
        {
            Vector3 targetPoint = hit.point + Vector3.up * vaultHeight;

            float heightDifference = hit.point.y - transform.position.y; // Height difference
            float vaultThreshold = 0.5f; // Allowable height difference to trigger vault

            if (heightDifference < vaultThreshold && !Physics.Raycast(targetPoint, Vector3.up, 1f)) // 1f = clearance height
            {
                StartCoroutine(VaultToPosition(targetPoint));
            }
        }
    }

    private IEnumerator VaultToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * vaultSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure exact final position
    }

    private void Crouch()
    {
        transform.localScale = crouchedScale; // Shrink player height
        playerCollider.size = new Vector3(playerCollider.size.x, crouchedColliderHeight, playerCollider.size.z); // Adjust collider height
        currentSpeed = moveSpeed * 0.5f; // Reduce speed while crouching
    }

    private void StandUp()
    {
        transform.localScale = originalScale; // Restore original size
        playerCollider.size = new Vector3(playerCollider.size.x, originalColliderHeight, playerCollider.size.z); // Restore original collider height
        currentSpeed = moveSpeed; // Restore original speed
    }

    private void ToggleNoclip()
    {
        isNoclip = !isNoclip;

        if (isNoclip)
        {
            rb.isKinematic = true; // Disable physics for free movement
            playerColliderComponent.enabled = false; // Disable collision
        }
        else
        {
            rb.isKinematic = false; // Re-enable physics
            playerColliderComponent.enabled = true; // Re-enable collision
        }
    }

    private void NoclipMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * noclipSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * noclipSpeed * Time.deltaTime;
        float moveY = 0f; // Lock vertical movement during noclip

        Vector3 move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        transform.position += move; // Free movement in any direction
    }
}
