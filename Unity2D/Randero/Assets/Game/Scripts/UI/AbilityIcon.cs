using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Randero.Combat;

namespace Randero.UI
{
    [RequireComponent(typeof(Image))]
    public class AbilityIcon : MonoBehaviour
    {
        public void SetAbility(Ability item)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetCardSprite();
            }
        }
    }
}
