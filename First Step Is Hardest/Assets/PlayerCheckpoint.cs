using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    public static PlayerCheckpoint instance;

    private Vector3 currentCheckpoint; // Stores the active checkpoint position

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpoint = checkpointPosition;
        Debug.Log("Checkpoint set at: " + currentCheckpoint);
    }

    private void Update()
    {
        // Teleport player to checkpoint when "F" is pressed
        if (Input.GetKeyDown(KeyCode.F) && currentCheckpoint != Vector3.zero)
        {
            TeleportToCheckpoint();
        }
    }

    private void TeleportToCheckpoint()
    {
        transform.position = currentCheckpoint;
        Debug.Log("Teleported to checkpoint: " + currentCheckpoint);
    }
}
