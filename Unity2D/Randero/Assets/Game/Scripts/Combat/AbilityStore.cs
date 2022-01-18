using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Randero.Combat
{
    public class AbilityStore : MonoBehaviour
    {
        [SerializeField] int abilityStoreSize = 3;

        Ability[] abilities = null;

        public event Action abilityStoreUpdated;

        private void Awake()
        {
            abilities = new Ability[abilityStoreSize];
        }

        public int GetSize()
        {
            return abilities.Length;
        }

        public bool AddToStore(Ability ability, int index)
        {
            int i = FindAvailableSlot();
            if (i < 0)
            {
                return false;
            }

            if (abilities[index] == null)
            {
                abilities[index] = ability;
            }
            else
            {
                abilities[i] = ability;
            }
            
            if (abilityStoreUpdated != null)
            {
                abilityStoreUpdated();
            }
            return true;
        }

        public void ClearStore()
        {
            abilities = new Ability[abilityStoreSize];
        }

        public Ability GetAbilityInSlot(int slot)
        {
            return abilities[slot];
        }

        public void RemoveAbilityFromSlot(int slot)
        {
            abilities[slot] = null;
            if (abilityStoreUpdated != null)
            {
                abilityStoreUpdated();
            }
        }

        private int FindAvailableSlot()
        {
            for (int i = 0; i < abilities.Length; ++i)
            {
                if (abilities[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
