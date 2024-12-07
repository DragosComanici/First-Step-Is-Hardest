using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCooldownAndDoubleJumpTrigger : MonoBehaviour
{
    public Movement playerMovement; // Reference to the player's movement script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player triggers this
        {
            if (playerMovement != null)
            {
                playerMovement.grappleCooldown = 0.1f; // Set the grapple cooldown to 0.1

                // Enable timed double jump every 0.5 seconds
                playerMovement.doubleJumpUnlocked = true;
                playerMovement.StartCoroutine(EnableDoubleJumpEveryInterval(playerMovement, 0.5f));

                Destroy(gameObject); // Remove the trigger object after activation
            }
        }
    }

    private IEnumerator EnableDoubleJumpEveryInterval(Movement playerMovement, float interval)
    {
        while (true)
        {
            playerMovement.hasDoubleJumped = false; // Reset double jump state
            yield return new WaitForSeconds(interval); // Wait for the specified interval
        }
    }
}
