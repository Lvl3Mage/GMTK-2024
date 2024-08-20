using System.Collections;
using System.Collections.Generic;
using Lvl3Mage.CameraManagement2D;
using UnityEngine;

public class MouseFollowBehaviour : MonoBehaviour
{
    Transform TR;

    void Start()
    {
        TR = GetComponent<Transform>();
    }

    void Update()
    {
        TR.position = SceneCamera.GetWorldMousePosition();
    }
}
