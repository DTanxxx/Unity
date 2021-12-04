using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3 : MonoBehaviour
{
    [SerializeField] private float rotationRate = 100.0f;
    [SerializeField] private float xRotationLimit = 90.0f;
    [SerializeField] private GameObject p = null;

    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void RotateX(float deltaTime, float deltaX)
    {
        transform.Rotate(0, deltaX * rotationRate * deltaTime, 0);
    }

    public void RotateY(float deltaTime, float deltaY)
    {
        var cameraTransform = p.transform.GetComponentInChildren<Camera>().transform;
        var newRotation = cameraTransform.localRotation.eulerAngles.x + -deltaY * rotationRate * deltaTime;
        if (newRotation >= xRotationLimit && newRotation <= (360.0f - xRotationLimit))
        {
            return;
        }
        cameraTransform.Rotate(-deltaY * rotationRate * deltaTime, 0, 0);
    }
}
