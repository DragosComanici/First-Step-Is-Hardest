using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Required for exiting play mode in the editor
#endif

public class FullscreenControl : MonoBehaviour
{
    void Start()
    {
        // Set the game to fullscreen mode
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
    }

    void Update()
    {
        // Check if the M key is pressed to exit play mode or application
        if (Input.GetKeyDown(KeyCode.M))
        {
            #if UNITY_EDITOR
            // Exit play mode in Unity Editor
            EditorApplication.isPlaying = false;
            #else
            // Quit the application in a build
            Application.Quit();
            #endif
        }
    }
}
