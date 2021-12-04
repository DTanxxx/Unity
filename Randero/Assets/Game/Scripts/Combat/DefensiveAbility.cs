using UnityEngine;
using Randero.Attribute;

namespace Randero.Combat
{
    [CreateAssetMenu(menuName = ("Randero/Combat/Defensive Ability"))]
    public class DefensiveAbility : Ability
    {
        [SerializeField] float healAmount = 10.0f;

        public override void UseAbility()
        {
            base.UseAbility();

            caster.RestoreHealth(healAmount);
        }
    }
}
