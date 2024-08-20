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
    string targetScene;
    public void AnimateToScene(string sceneName)
    {
        transition.SetTrigger("start");
    }
    public void SwitchToTargetScene()
    {
        SceneManager.LoadScene(targetScene);
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
        SceneManager.LoadScene("Tutorial");
    }

    public void exit()
    {
        Application.Quit();
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
