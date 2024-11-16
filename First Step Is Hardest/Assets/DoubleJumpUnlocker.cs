using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpUnlocker : MonoBehaviour
{
    // Reference to the player's Movement script
    public Movement playerMovement;

    // Message to show that the double jump has been unlocked
    public string unlockMessage = "Double Jump Unlocked!";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Ensure the playerMovement is not null
            if (playerMovement != null)
            {
                // Unlock double jump
                playerMovement.UnlockDoubleJump();

                // Optional: Show a message or feedback
                Debug.Log(unlockMessage);

                // Destroy this unlocker object so it can't be used again (optional)
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player movement script reference is missing.");
            }
        }
    }
}
