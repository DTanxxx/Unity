using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Randero.Attribute;
using UnityEngine.UI;

namespace Randero.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] Health playerHealth = null;
        [SerializeField] GameObject foregroundObject = null;

        private void Update()
        {
            float foregroundScale = playerHealth.GetPlayerHealth() / playerHealth.GetMaxHealth();
            foregroundObject.GetComponent<RectTransform>().localScale = new Vector3(foregroundScale, 1, 1);
        }
    }
}
