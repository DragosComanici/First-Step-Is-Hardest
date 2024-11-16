using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 2.0f;  // Mouse sensitivity
    public float clampAngle = 80.0f;  // Maximum vertical angle

    private float rotationX = 0.0f;    // Vertical rotation
    private float rotationY = 0.0f;    // Horizontal rotation

    void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Update rotation values
        rotationX -= mouseY; // Invert the Y axis
        rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle); // Clamp vertical rotation

        rotationY += mouseX;

        // Apply rotations to the camera
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
}
