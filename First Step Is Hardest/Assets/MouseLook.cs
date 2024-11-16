using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f; // General sensitivity for both axes
    public float rotationSmoothness = 0.1f; // Smoothness of rotation (lower = smoother)

    [Header("Clamping Settings")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    public float clampSoftness = 0.8f; // Soft clamping adjustment (0-1)

    public Transform playerBody;

    private float xRotation = 0f; // Vertical rotation
    private float smoothXVelocity; // For smoothing vertical rotation
    private float smoothYVelocity; // For smoothing horizontal rotation

    private Vector2 currentMouseDelta; // Smoothed mouse delta
    private Vector2 currentMouseDeltaVelocity; // Velocity used by SmoothDamp

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor
    }

    void Update()
    {
        HandleMouseLook();
    }

    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    private void HandleMouseLook()
    {
        // Raw mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Smooth the mouse input
        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, rotationSmoothness);

        // Adjust and clamp vertical rotation
        xRotation -= currentMouseDelta.y; // Invert Y-axis for natural look
        xRotation = Mathf.Lerp(xRotation, Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle), clampSoftness);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical rotation
        playerBody.Rotate(Vector3.up * currentMouseDelta.x); // Horizontal rotation
    }
}
