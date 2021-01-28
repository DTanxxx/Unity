using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 100;
    int damageDueToShipCollision = 200;

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        if (damage != damageDueToShipCollision)
        {
            Destroy(gameObject);
        }       
    }
}
