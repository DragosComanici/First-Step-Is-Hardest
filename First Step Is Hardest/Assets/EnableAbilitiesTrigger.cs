using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAbilitiesTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Access the player's movement script
            Movement playerMovement = other.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.doubleJumpUnlocked = true; // Re-enable double jump
                playerMovement.grappleCooldown = 2f; // Reset grapple cooldown to its original value (adjust as needed)
            }

            // Optionally, make this trigger disappear after activation
            gameObject.SetActive(false);
        }
    }
}
