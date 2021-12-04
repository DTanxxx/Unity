using GameDevTV.Inventories;
using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Health Action Item"))]
    public class HealthActionItem : ActionItem
    {
        [SerializeField] float healthToRestore = 50.0f;

        public override void Use(GameObject user)
        {
            Health playerHealth = user.GetComponent<Health>();
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.Heal(healthToRestore);
        }
    }
}
