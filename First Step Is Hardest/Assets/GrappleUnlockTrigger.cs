using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleUnlockTrigger : MonoBehaviour
{
    public GameObject player; // Reference to the player
    private Movement playerMovementScript; // Reference to the player's movement script

    public float interactDistance = 3f; // Distance within which the player can interact with the object
    public KeyCode interactKey = KeyCode.E; // Key to interact with the object

    void Start()
    {
        // Find and store the player's movement script
        playerMovementScript = player.GetComponent<Movement>();
    }

    void Update()
    {
        // Check if the player is close enough and presses the interact key
        if (Vector3.Distance(player.transform.position, transform.position) <= interactDistance && Input.GetKeyDown(interactKey))
        {
            UnlockGrappleLock(); // Unlock the grapple lock mechanic
        }
    }

    void UnlockGrappleLock()
    {
        if (playerMovementScript != null)
        {
            // Unlock the grapple lock mechanic in the player's movement script
            playerMovementScript.grappleLockUnlocked = true;

            // Optional: Provide feedback to the player, such as a message or sound
            Debug.Log("Grapple Lock Mechanic Unlocked!");

            // You can add additional visual or sound effects here if desired
        }
    }
}
