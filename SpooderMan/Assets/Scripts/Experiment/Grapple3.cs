using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple3 : MonoBehaviour
{
    [SerializeField] private Camera cam = null;
    [SerializeField] private float ropeLength = 15.0f;
    [SerializeField] private LayerMask layer = new LayerMask();
    [SerializeField] private LineRenderer lineRenderer = null;
    [SerializeField] private GameObject spawnPoint = null;
    [SerializeField] private float springForce = 25.0f;
    [SerializeField] private float damperForce = 1.5f;
    [SerializeField] private float massScale = 4.5f;
    [SerializeField] private GameObject redDot = null;

    SpringJoint springJoint;
    RaycastHit hit;
    GameObject guideDot;

    private void Start()
    {
        guideDot = Instantiate(redDot, Vector3.zero, Quaternion.identity);
        guideDot.SetActive(false);
    }

    private void Update()
    {
        if (lineRenderer.positionCount > 0)
        {
            // update line rendered
            Vector3[] newPos = { hit.point, spawnPoint.transform.position };
            lineRenderer.SetPositions(newPos);
        }
    }

    public void ShowGrappleGuide()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit guideHit, ropeLength, layer))
        {
            // hit a block
            // if a rope is active, don't move the guide dot
            if (lineRenderer.positionCount > 0)
            {
                return;
            }
            guideDot.SetActive(true);
            guideDot.transform.position = guideHit.point;
        }       
        else if (lineRenderer.positionCount == 0)
        {
            // didn't hit anything and no rope then diable dot
            guideDot.SetActive(false);
        }
    }

    public void StartGrapple()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, ropeLength, layer))
        {
            // hit something
            // add a spring joint to player
            springJoint = gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = hit.point;

            float distanceFromHook = Vector3.Distance(hit.point, transform.position);
            springJoint.maxDistance = distanceFromHook * 0.8f;
            springJoint.minDistance = distanceFromHook * 0.2f;

            springJoint.spring = springForce;
            springJoint.damper = damperForce;
            springJoint.massScale = massScale;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = 2;
            Vector3[] positions = { hit.point, spawnPoint.transform.position };
            lineRenderer.SetPositions(positions);
        }
    }

    public void EndGrapple()
    {
        // remove rendered line
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(new Vector3[0]);
        // remove spring joint
        Destroy(springJoint);
    }
}
