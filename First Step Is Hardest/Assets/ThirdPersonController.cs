using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f;
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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        bool isMoving = direction.magnitude > 0.1f;

        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 move = direction * speed * Time.deltaTime;
            characterController.Move(move);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (!characterController.isGrounded)
        {
            Vector3 gravity = new Vector3(0, -9.81f, 0) * Time.deltaTime;
            characterController.Move(gravity);
        }
    }
}
