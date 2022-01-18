using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (other.GetComponent<Hook>())
        {
            Debug.Log("Hit block");
            // segment hit a block
            // stop extension
            transform.parent.parent.GetComponentInChildren<GrappleGun>().stopExtending = true;
            // attach last segment to block
            transform.parent.GetComponent<HingeJoint>().connectedBody = other.GetComponent<Rigidbody>();
            //transform.parent.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, transform.localScale.y * 2
                //* transform.parent.localScale.z);
            //transform.parent.GetComponent<HingeJoint>().connectedAnchor = new Vector3(0, 0,
                //0);
            /*FindObjectOfType<GrappleGun>().GetComponent<HingeJoint>().
                connectedBody = other.GetComponent<Rigidbody>();*/

            transform.parent.GetComponent<Rigidbody>().useGravity = true;
            transform.parent.GetComponent<Rigidbody>().isKinematic = false;

            // disable mouse movement on player controller
            FindObjectOfType<PlayerController>().isHanging = true;
            FindObjectOfType<PlayerController>().GetComponent<Rigidbody>().useGravity = false;
            FindObjectOfType<PlayerController>().GetComponent<Rigidbody>().isKinematic = true;

            FindObjectOfType<GrappleGun>().GetComponent<Rigidbody>().useGravity = true;
            FindObjectOfType<GrappleGun>().GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
