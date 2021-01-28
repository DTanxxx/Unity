using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] float standingThreshold = 10f;
    [SerializeField] float distanceToRaise = 50f;

    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        IsStanding();
    }

    public bool IsStanding()
    {
        Vector3 rotationInEuler = transform.rotation.eulerAngles;

        float tiltInX = Mathf.Abs(rotationInEuler.x);
        float tiltInZ = Mathf.Abs(rotationInEuler.z);
        if (tiltInX > 180) { tiltInX = 360 - tiltInX; }
        if (tiltInZ > 180) { tiltInZ = 360 - tiltInZ; }

        if (tiltInX <= standingThreshold && tiltInZ <= standingThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RaiseIfStanding()
    {
        if (IsStanding())
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            rigidBody.useGravity = false;
            transform.Translate(new Vector3(0f, distanceToRaise, 0f), Space.World);
        }
    }

    public void Lower()
    {
        transform.Translate(new Vector3(0f, -distanceToRaise, 0f), Space.World);
        rigidBody.useGravity = true;
    }
}
