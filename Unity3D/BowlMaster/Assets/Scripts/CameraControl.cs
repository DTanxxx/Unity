using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Ball ball;

    private Vector3 offSet;

    void Start()
    {
        offSet = transform.position - ball.transform.position;
    }

    void Update()
    {
        if (ball.transform.position.z <= 1829f)
        {
            // still in front of head pin, change camera's position
            transform.position = ball.transform.position + offSet;
        }
    }
}
