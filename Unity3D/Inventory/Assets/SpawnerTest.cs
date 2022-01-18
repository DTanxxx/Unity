using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.UI.Inventories;
using GameDevTV.Inventories;

public class SpawnerTest : MonoBehaviour
{
    [SerializeField] ItemTooltip tooltipPrefab = null;

    void Start()
    {
        ItemTooltip tooltip = Instantiate(tooltipPrefab, transform);
        tooltip.Setup(InventoryItem.GetFromID("8baecbea-2962-4ce2-80c9-94b24917f4cb"));
    }
}
