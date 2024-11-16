using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLock : MonoBehaviour
{
    public float lockRange = 50f;  // How far the player can lock onto an object
    public float lockSpeed = 5f;   // Speed at which the player moves around the sphere
    public float maxDistanceFromObject = 5f; // Maximum distance the player can be from the object
    private bool isLocked = false; // To track whether the player is locked onto an object
    private Vector3 lockPosition;  // Position the player is locked onto
    private GameObject lockedObject;  // The object the player is locked onto
    private Vector3 lockCenter;  // The center of the lock (the object)
    private float radius;  // Radius of the locking sphere

    private Rigidbody rb;
    private Camera playerCamera;
    private Collider playerCollider; // The player's collider

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        playerCollider = GetComponent<Collider>();

        // Ensure the Rigidbody is using Interpolate and Collision Detection for smoother movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            LockOntoObject();
        }

        if (Input.GetMouseButton(0) && isLocked) // While holding left-click
        {
            MoveAroundSphere();
        }

        if (Input.GetMouseButtonUp(0)) // When left-click is released
        {
            Unlock();
        }
    }

    void LockOntoObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lockRange))
        {
            if (hit.collider != null)
            {
                lockedObject = hit.collider.gameObject;  // The object we're locking onto
                lockCenter = lockedObject.transform.position;  // The position of the locked object
                radius = Vector3.Distance(lockCenter, transform.position);  // Distance between player and object

                isLocked = true;  // Lock the player onto the object
                rb.useGravity = false;  // Disable gravity while locked onto the object
                rb.velocity = Vector3.zero;  // Zero out any existing velocity to prevent drifting
                playerCollider.isTrigger = false; // Ensure collisions are handled correctly
            }
        }
    }

    void MoveAroundSphere()
    {
        if (lockedObject != null)
        {
            // Get input for rotating the player
            float horizontalInput = Input.GetAxis("Horizontal");  // A/D or Arrow keys for rotation
            float verticalInput = Input.GetAxis("Vertical");      // W/S or Arrow keys for up/down movement

            // Calculate the direction the player is moving
            Vector3 direction = (transform.position - lockCenter).normalized;  // Direction from the locked object
            Vector3 right = Vector3.Cross(Vector3.up, direction);  // Right direction for horizontal movement
            Vector3 up = Vector3.Cross(direction, right);  // Up direction for vertical movement

            // Apply the movement (flip horizontalInput to correct the direction)
            Vector3 moveDirection = right * -horizontalInput + up * verticalInput;

            // Ensure the player stays on the surface of the sphere
            Vector3 newPosition = lockCenter + direction * radius + moveDirection * lockSpeed * Time.deltaTime;
            transform.position = newPosition;

            // Keep the player upright
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Keep the player looking at the locked object
            transform.LookAt(lockedObject.transform);
        }
    }

    void Unlock()
    {
        isLocked = false;  // Unlock the player
        rb.useGravity = true;  // Re-enable gravity when unlocking
        playerCollider.isTrigger = false; // Restore normal collisions
    }
}
