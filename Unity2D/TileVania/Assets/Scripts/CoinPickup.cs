using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int pointsForCoinPickup = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject as GameObject;
        CapsuleCollider2D playerBody = player.GetComponent<CapsuleCollider2D>();
        if (!playerBody.IsTouchingLayers(LayerMask.GetMask("Interactables")))
        {
            return;
        }

        AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
        FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
        Destroy(gameObject);
    }
}
