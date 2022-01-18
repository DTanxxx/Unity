using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Randero.Modifier;
using System;

namespace Randero.Attribute
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100.0f;

        float currentHealth = 0.0f;
        List<Effect> activeEffects = new List<Effect>();

        public static event Action<bool> onApplyFreeze;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(currentHealth - damageAmount, 0.0f);
            Debug.Log($"{gameObject.name} took {damageAmount} damage");
            if (currentHealth <= 0.0f)
            {
                // Player is dead!
                Debug.Log($"{gameObject.name} is dead!");
            }   
        }

        public void ApplyContinuingEffect()
        {
            if (activeEffects.Count == 0)
            {
                return;
            }

            List<Effect> updatedActiveEffects = new List<Effect>();
            
            // iterate through effects and apply them
            foreach (var effect in activeEffects)
            {
                // check if effect has remaining turns = 0
                if (effect.continuingDamageTurns == 0)
                {
                    if (effect.doesFreeze)
                    {
                        Debug.Log("DeFreeze!");
                        // this relies on activeEffects being in order (earlier effects are at the front of list)
                        onApplyFreeze?.Invoke(false);                       
                    }
                    continue;
                }

                var damage = effect.damage;
                var continuingDamage = damage * effect.continuingDamagePercentage / 100.0f;
                TakeDamage(continuingDamage);

                if (effect.doesFreeze)
                {
                    Debug.Log("Freeze");
                    onApplyFreeze?.Invoke(true);
                }

                if (effect.continuingDamageTurns >= 1)
                {
                    Effect updatedEffect = new Effect();
                    updatedEffect.damage = effect.damage;
                    updatedEffect.doesFreeze = effect.doesFreeze;
                    updatedEffect.continuingDamagePercentage = effect.continuingDamagePercentage;
                    updatedEffect.continuingDamageTurns = effect.continuingDamageTurns - 1;
                    updatedActiveEffects.Add(updatedEffect);
                }         
            }

            activeEffects = updatedActiveEffects;
        }

        public void RestoreHealth(float healAmount)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, 100.0f);

            Debug.Log($"{gameObject.name} restored {healAmount} health");
        }

        public void AddEffectOverTurn(Effect effect)
        {
            activeEffects.Add(effect);
        }

        public float GetPlayerHealth()
        {
            return currentHealth;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
