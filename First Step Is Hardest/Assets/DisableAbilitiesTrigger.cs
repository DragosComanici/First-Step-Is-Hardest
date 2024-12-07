using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAbilitiesTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Access the player's movement script
            Movement playerMovement = other.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.doubleJumpUnlocked = false; // Disable double jump
                playerMovement.grappleCooldown = Mathf.Infinity; // Disable grapple by setting an infinite cooldown
            }

            // Optionally, make this trigger disappear after activation
            gameObject.SetActive(false);
        }
    }
}
