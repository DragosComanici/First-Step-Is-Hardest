using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            // Save this checkpoint's position as the player's respawn point
            PlayerCheckpoint.instance.SetCheckpoint(transform.position);

            // Disable the checkpoint object
            gameObject.SetActive(false);
        }
    }
}
