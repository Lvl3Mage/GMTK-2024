using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelLoader : MonoBehaviour
{
    public Animator transition;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            if (Input.anyKeyDown)
            {
                transition.SetTrigger("main");
                //LoadMainMenu();
            }
        }
    }

    public void callForRestart()
    {
        transition.SetTrigger("restart");
    }

    public void callForChangeScene()
    {
        transition.SetTrigger("start");
    }

    public void callForChangeCredits()
    {
        transition.SetTrigger("credits");
    }

    public void LoadRestart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
