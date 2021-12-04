using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Randero.Combat;

namespace Randero.UI
{
    public class AbilityBarUI : MonoBehaviour
    {
        [SerializeField] AbilitySlotUI abilitySlotPrefab = null;

        AbilityStore abilityStore = null;

        public void Initialise(AbilityStore store)
        {
            abilityStore = store;
            abilityStore.abilityStoreUpdated += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < abilityStore.GetSize(); ++i)
            {
                var abilityUI = Instantiate(abilitySlotPrefab, transform);
                abilityUI.Setup(abilityStore, i);
            }
        }
    }
}
