using System.Collections;
using System.Collections.Generic;
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
        TR.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
