using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class particleScene : MonoBehaviour
{
    [SerializeField]ParticleSystem particles;

    private void Start()
    {
        SceneManager.sceneLoaded += DisableParticleSystem; 
    }

    void DisableParticleSystem(Scene scene, LoadSceneMode loadSceneMode)
    {
        particles.Stop();
    }
}
