using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private bool tethered = false;
    public float tetherLength = 0.0f;
    public Vector3 tetherPoint;
    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!tethered)
            {
                BeginGrapple();
            }
            else
            {
                EndGrapple();
            }
        }
    }

    private void FixedUpdate()
    {
        if (tethered)
        {
            ApplyGrapplePhysics();
        }
    }

    private void BeginGrapple()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            tethered = true;
            tetherPoint = hit.point;
            tetherLength = Vector3.Distance(hit.point, transform.position);
        }
    }

    private void EndGrapple()
    {
        tethered = false;
    }

    private void ApplyGrapplePhysics()
    {
        Vector3 directionToGrapple = Vector3.Normalize(tetherPoint - transform.position);
        float currentDistanceToGrapple = Vector3.Distance(tetherPoint, transform.position);

        float speedTowardsGrapplePoint = Mathf.Round(Vector3.Dot(body.velocity, directionToGrapple) * 100) / 100;

        if (speedTowardsGrapplePoint < 0)
        {
            if (currentDistanceToGrapple > tetherLength)
            {
                body.velocity -= speedTowardsGrapplePoint * directionToGrapple;
                body.position = tetherPoint - directionToGrapple * tetherLength;
            }
        }
    }
}
