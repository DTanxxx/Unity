using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float rotationRate = 100.0f;
    [SerializeField] private float jumpStrength = 10.0f;
    [SerializeField] private GameObject rotationPivot = null;

    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void RotateX(float deltaTime, float deltaX)
    {
        rotationPivot.transform.Rotate(0, deltaX * rotationRate * deltaTime, 0);
    }

    public void RotateY(float deltaTime, float deltaY)
    {
        transform.parent.transform.Rotate(-deltaY * rotationRate * deltaTime, 0, 0);
    }

    public void Jump()
    {
        rigidBody.AddForce(Vector3.up * jumpStrength);
    }
}
