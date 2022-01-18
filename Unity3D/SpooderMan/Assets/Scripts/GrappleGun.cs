using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    [SerializeField] private GameObject grappleRope = null;
    [SerializeField] private GameObject ropeSpawnPoint = null;
    [SerializeField] private float deltaRopeSegmentLength = 50.0f;
    [SerializeField] private float maxRopeSegmentLength = 50.0f;
    [SerializeField] private int ropeSegmentNumber = 10;

    private int currentRopeSegmentIndex = -1;
    private float currentRopeSegmentLength = 0.0f;
    private GameObject[] ropeSegments;
    private float segmentSpawnZPosition;
    private float ropeLengthSoFar = 0.0f;
    public bool stopExtending = false;

    private void Start()
    {
        ropeSegments = new GameObject[ropeSegmentNumber];
    }

    public void Fire()
    {
        Debug.Log("Fired");
        // instantiate grappleRope and assign to ropeInstances array        
        segmentSpawnZPosition = CalculateSegmentSpawnPosition();
        var ropeInstance = Instantiate(grappleRope, transform.parent);

        // TODO
        // if first segment then don't do anything
        // if not then assign rigidbody in hinge joint component with previous segment
        // if last segment then assign this player's rigidbody in hinge joint with last segment


        ropeInstance.transform.localPosition = new Vector3(0, 0, segmentSpawnZPosition);
        currentRopeSegmentIndex += 1;
        ropeSegments[currentRopeSegmentIndex] = ropeInstance;

 /*       if (currentRopeSegmentIndex > 0)
        {
            // more than one segment, so assign connected body of previous seg as this seg
            ropeSegments[currentRopeSegmentIndex - 1].GetComponentInChildren<HingeJoint>().connectedBody =
                ropeSegments[currentRopeSegmentIndex].GetComponentInChildren<Rigidbody>();
        }
        */if (currentRopeSegmentIndex == 0)
        {
            // first segment, connected body of player is this seg
            GetComponent<HingeJoint>().connectedBody = 
                ropeSegments[currentRopeSegmentIndex].GetComponent<Rigidbody>();
        }
    }

    public void DestroyRope()
    {
        Debug.Log("Rope Destroyed");

        for (int i = 0; i < ropeSegments.Length; ++i)
        {
            Destroy(ropeSegments[i]);
            ropeSegments[i] = null;
        }
        currentRopeSegmentIndex = -1;
        currentRopeSegmentLength = 0.0f;
        ropeLengthSoFar = 0.0f;
    }

    private void Update()
    {
        if (currentRopeSegmentIndex < 0) { return; }
        if (stopExtending) { return; }
        if (currentRopeSegmentLength > maxRopeSegmentLength && currentRopeSegmentIndex == ropeSegmentNumber - 1)
        {
            // destroy rope if the last segment has reached max length
            DestroyRope();
            return;
        }

        if (currentRopeSegmentLength > maxRopeSegmentLength)
        {
            // spawn a new segment
            currentRopeSegmentLength = 0.0f;
            Fire();
            return;
        }

/*        if (!Input.GetMouseButton(0))
        {
            // destroy rope
            return;
        }*/

        ropeSegments[currentRopeSegmentIndex].transform.localScale = 
            new Vector3(ropeSegments[currentRopeSegmentIndex].transform.localScale.x,
            ropeSegments[currentRopeSegmentIndex].transform.localScale.y, currentRopeSegmentLength);
        currentRopeSegmentLength += Time.deltaTime * deltaRopeSegmentLength;
    }

    private float CalculateSegmentSpawnPosition()
    {
        // calculate new spawn position
        if (currentRopeSegmentIndex < 0)
        {
            return ropeSpawnPoint.transform.localPosition.z;
        }

        var meshFilter = ropeSegments[currentRopeSegmentIndex].GetComponentInChildren<MeshFilter>();
        var mesh = meshFilter.mesh;
        ropeLengthSoFar += mesh.bounds.size.y * meshFilter.transform.localScale.y *
            ropeSegments[currentRopeSegmentIndex].transform.localScale.z;

        return ropeSpawnPoint.transform.localPosition.z + ropeLengthSoFar;
    }
}
