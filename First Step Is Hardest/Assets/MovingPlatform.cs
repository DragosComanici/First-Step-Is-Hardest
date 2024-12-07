using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // Starting point
    public Transform pointB; // Ending point
    public float speed = 2f; // Speed of movement
    private Vector3 targetPosition; // Current target position

    private void Start()
    {
        // Start moving towards PointA
        targetPosition = pointA.position;
    }

    private void Update()
    {
        // Move the platform towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the platform has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Switch the target position to the other point
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }
}
