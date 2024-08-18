using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelLoader : MonoBehaviour
{
    public Animator transition;
    //private void Update()
    //{
    //    Debug.Log(transform.position);

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //    }
    //}

    public void callForChangeScene()
    {
        transition.SetTrigger("start");
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
