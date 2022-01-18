using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Randero.Combat
{
    [CreateAssetMenu(menuName = ("Randero/Combat/CharacterClass"))]
    public class CharacterClass : ScriptableObject
    {
        [SerializeField] Ability[] abilities = null;
        [SerializeField] Sprite characterSprite = null;

        Ability[] clonedAbilities;

        public Ability[] GetAbilities()
        {
            if (clonedAbilities == null)
            {
                Debug.LogError("NULL abilities");
            }
            return clonedAbilities;
        }

        public Sprite GetCharacterSprite()
        {
            return characterSprite;
        }

        public void CloneAbilities()
        {
            var temp = new List<Ability>();
            foreach (var ability in abilities)
            {
                temp.Add(Instantiate(ability));
            }
            clonedAbilities = temp.ToArray();
        }
    }
}
