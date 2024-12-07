using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float disappearTime = 3f; // Time before the platform disappears
    public float reappearTime = 5f;  // Time before the platform reappears after disappearing
    public float startDelay = 2f;    // Delay before the disappearing and reappearing starts

    private MeshRenderer meshRenderer;
    private Collider platformCollider;

    private void Start()
    {
        // Get the MeshRenderer and Collider components
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();

        // Start the cycle with the delay
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for the start delay before beginning the platform cycle
        yield return new WaitForSeconds(startDelay);

        // Start the cycle
        StartCoroutine(PlatformCycle());
    }

    private IEnumerator PlatformCycle()
    {
        while (true) // Loop the cycle indefinitely
        {
            // Platform is visible and active
            EnablePlatform();
            yield return new WaitForSeconds(disappearTime); // Wait for the disappear time

            // Platform is invisible and inactive
            DisablePlatform();
            yield return new WaitForSeconds(reappearTime); // Wait for the reappear time
        }
    }

    private void EnablePlatform()
    {
        meshRenderer.enabled = true; // Make the platform visible
        platformCollider.enabled = true; // Enable collisions
    }

    private void DisablePlatform()
    {
        meshRenderer.enabled = false; // Make the platform invisible
        platformCollider.enabled = false; // Disable collisions
    }
}
