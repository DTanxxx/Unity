using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Randero.Combat;
using Randero.Utils;

namespace Randero.UI
{
    public class AbilitySlotUI : MonoBehaviour, IDragContainer<Ability>
    {
        [SerializeField] AbilityIcon icon = null;

        int index = 0;
        AbilityStore abilityStore = null;

        public void Setup(AbilityStore store, int index)
        {
            abilityStore = store;
            this.index = index;

            if (icon != null)
            {
                icon.SetAbility(abilityStore.GetAbilityInSlot(this.index));
            }
        }

        public Ability GetItem()
        {
            return abilityStore.GetAbilityInSlot(index);
        }

        public void RemoveItem()
        {
            abilityStore.RemoveAbilityFromSlot(index);
        }

        public void AddItem(Ability item)
        {
            abilityStore.AddToStore(item, index);
        }
    }
}
