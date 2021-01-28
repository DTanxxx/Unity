using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
    public bool recording = true;

    private float fixedDeltaTime;
    private bool isPaused = false;

    private void Start()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButton("Fire1"))
        {
            recording = false;
        }
        else
        {
            recording = true;
        }

        if (Input.GetKeyDown(KeyCode.P) && isPaused)
        {
            isPaused = false;
            ResumeGame();
        }
        else if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            isPaused = true;
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;        
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTime;               
    }

    private void OnApplicationPause(bool pause)
    {
        isPaused = pause;
    }
}
