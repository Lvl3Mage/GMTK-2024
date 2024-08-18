using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    float defaultFixedDelta, defaultTimeScale;

    bool isPaused;

    void Start()
    {
        defaultFixedDelta = Time.fixedDeltaTime;
        defaultTimeScale = Time.timeScale;
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (isPaused) PausedUpdate();
        else ResumedUpdate();
    }

    void ResumedUpdate()
    {
        //Updates when game is not paused:
    }

    void PausedUpdate()
    {
        //Updates when game is paused:
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;

        //PauseEffects:
        defaultFixedDelta = Time.fixedDeltaTime;
        defaultTimeScale = Time.timeScale;
        Time.fixedDeltaTime = 0f;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        
        //ResumeEffects:
        Time.fixedDeltaTime = defaultFixedDelta;
        Time.timeScale = defaultTimeScale;
    }
}
