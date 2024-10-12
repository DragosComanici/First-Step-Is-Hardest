using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
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
    private Rigidbody rb;

    private float lastGrappleTime = -Mathf.Infinity; // Track last grapple time

    // Crouching variables
    private Vector3 originalScale;
    private Vector3 crouchedScale; // Height reduced but same width and depth
    private BoxCollider playerCollider; // Reference to the player's collider
    public float crouchedColliderHeight = 2f; // Height of collider when crouching
    public float originalColliderHeight; // Original height of the collider

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody
        playerCollider = GetComponent<BoxCollider>(); // Get the BoxCollider component
        originalScale = transform.localScale; // Store original scale
        originalColliderHeight = playerCollider.size.y; // Store original collider height

        // Set crouched scale to maintain width and depth while reducing height
        crouchedScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z); // Half the height
    }

    // Update is called once per frame
    void Update()
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

        // Movement
        float moveX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveX, 0, moveZ));

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Grapple mechanic with cooldown check
        if (Input.GetMouseButtonDown(1) && Time.time >= lastGrappleTime + grappleCooldown)
        {
            Grapple();
        }

        // Vaulting mechanic
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded) // Only vault if not grounded
        {
            AttemptVault();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Grapple functionality
    void Grapple()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something within maxGrappleDistance
        if (Physics.Raycast(ray, out hit, maxGrappleDistance))
        {
            if (hit.collider != null)
            {
                Vector3 grapplePoint = hit.point; // Point where the ray hit
                ApplyGrappleForce(grapplePoint);
                lastGrappleTime = Time.time; // Record the time of this grapple
            }
        }
    }

    void ApplyGrappleForce(Vector3 targetPoint)
    {
        // Calculate direction toward the grapple point
        Vector3 direction = (targetPoint - transform.position).normalized;

        // Apply force in that direction
        rb.AddForce(direction * grappleForce, ForceMode.Impulse);
    }

    private void AttemptVault()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 rayOrigin = transform.position + Vector3.up * (originalColliderHeight / 2); // Start ray a bit above player's feet

        // Shoot a ray forward and down to check for the edge
        if (Physics.Raycast(rayOrigin, forward, out hit, vaultDistance))
        {
            // Calculate the vault height position
            Vector3 targetPoint = hit.point + Vector3.up * vaultHeight;

            // Check if the player's position is close enough to the edge
            float heightDifference = hit.point.y - transform.position.y; // Height difference
            float vaultThreshold = 0.5f; // Allowable height difference to trigger vault (adjust as needed)

            // Trigger vault if within the threshold and there is enough clearance above
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

        // Smoothly move the player to the vault position
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * vaultSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure exact final position
    }

    // Crouch functionality
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
}
