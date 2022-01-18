using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace InventoryExample.Control
{
    [RequireComponent(typeof(Pickup))]
    public class RunoverPickup : MonoBehaviour
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();    
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                pickup.PickupItem();
            }
        }
    }
}
