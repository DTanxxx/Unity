using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    [SerializeField] private GameObject experimentContainerObject = null;
    [SerializeField] private GameObject blockContainer = null;
    [SerializeField] private float overlapRadius = 5.0f;
    [SerializeField] private float power = 5.0f;
    [SerializeField] private float upwardsForce = 5.0f;
    [SerializeField] private ForceMode mode = ForceMode.Impulse;

    private GameObject pieces = null;

    private IEnumerator RemovePieces(GameObject pieces)
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(pieces);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        
        if (collision.gameObject.GetComponent<Bump>() && pieces == null)
        {
            Debug.Log("HIT");
            Destroy(gameObject);
            // is player
            pieces = Instantiate(blockContainer, transform.position, transform.rotation, experimentContainerObject.transform);
            pieces.transform.localScale = transform.localScale;

            ContactPoint[] contacts = new ContactPoint[collision.contactCount];
            collision.GetContacts(contacts);
            Vector3 explosionPos = Vector3.zero;

            foreach (var point in contacts)
            {
                if (point.otherCollider.gameObject.GetComponent<Bump>()
                    || point.thisCollider.gameObject.GetComponent<Bump>())
                {
                    // is player contact point
                    explosionPos = point.point;
                    break;
                }
            }

            Collider[] colliders = Physics.OverlapSphere(explosionPos, overlapRadius);

            foreach (Collider hit in colliders)
            {
                if (!hit.GetComponent<Bump>() && !hit.GetComponent<PlayerController3>() && hit.GetComponent<Rigidbody>())
                {
                    // overlapped broken pieces
                    hit.GetComponent<Rigidbody>().AddExplosionForce(power * collision.relativeVelocity.magnitude, explosionPos, overlapRadius, upwardsForce);
                }
            }

            StartCoroutine(RemovePieces(pieces));
        }
    }
}
