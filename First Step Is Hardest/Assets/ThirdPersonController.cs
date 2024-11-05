using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float rotationSpeed = 720f; // Rotation speed
    private CharacterController characterController;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Check if the character is moving
        bool isMoving = direction.magnitude > 0.1f;

        // Update animator state
        animator.SetBool("isRunning", isMoving);

        // If moving, handle movement
        if (isMoving)
        {
            // Calculate target rotation and move direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move character
            Vector3 move = direction * speed * Time.deltaTime;
            characterController.Move(move);
        }
        else
        {
            // Reset animation if not moving
            animator.SetBool("isRunning", false);
        }

        // Handle gravity
        if (!characterController.isGrounded)
        {
            Vector3 gravity = new Vector3(0, -9.81f, 0) * Time.deltaTime;
            characterController.Move(gravity);
        }
    }
}
