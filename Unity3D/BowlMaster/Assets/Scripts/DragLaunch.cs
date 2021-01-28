using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Ball))]
public class DragLaunch : MonoBehaviour
{
    private Ball ball;
    private Vector3 dragStart, dragEnd;
    private float startTime, endTime;

    void Start()
    {
        ball = GetComponent<Ball>();
    }

    public void MoveStart(float amount)
    {
        if (!ball.inPlay)
        {
            float zPos = ball.transform.position.z;
            float yPos = ball.transform.position.y;
            float xPos = Mathf.Clamp(ball.transform.position.x + amount, -50, 50);
            ball.transform.position = new Vector3(xPos, yPos, zPos);
        }       
    }

    public void DragStart()
    {
        if (!ball.inPlay)
        {
            // Capture time & position of drag start
            dragStart = Input.mousePosition;
            startTime = Time.time;
        }       
    }

    public void DragEnd()
    {
        if (!ball.inPlay)
        {
            // Launch the ball
            dragEnd = Input.mousePosition;
            endTime = Time.time;

            float dragDuration = endTime - startTime;

            float launchSpeedX = (dragEnd.x - dragStart.x) / dragDuration;
            float launchSpeedZ = (dragEnd.y - dragStart.y) / dragDuration;

            Vector3 launchVelocity = new Vector3(launchSpeedX, 0f, launchSpeedZ);

            ball.Launch(launchVelocity);
        }       
    }
}
