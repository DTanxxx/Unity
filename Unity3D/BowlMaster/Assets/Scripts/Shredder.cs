using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameObject objectExit = other.gameObject;

        if (objectExit.GetComponentInParent<Pin>())
        {
            Destroy(objectExit.transform.parent.gameObject);
        }
    }
}
