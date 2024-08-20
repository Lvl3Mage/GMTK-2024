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

    public void LoadTutorial()
    {
        LoadSceneAsync("Tutorial");
    }

    public void exit()
    {
        Application.Quit();
    }

    public void LoadRestart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadSceneAsync(currentSceneName);
    }
    public void LoadNextLevel()
    {
        LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCredits()
    {
        LoadSceneAsync("Credits");
    }

    public void LoadMainMenu()
    {
        LoadSceneAsync("MainMenu");
    }
    public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
    public void LoadSceneAsync(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
