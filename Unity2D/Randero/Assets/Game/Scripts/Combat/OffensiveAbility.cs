using UnityEngine;
using Randero.Modifier;

namespace Randero.Combat
{
    [CreateAssetMenu(menuName = ("Randero/Combat/Offensive Ability"))]
    public class OffensiveAbility : Ability
    {
        [SerializeField] Effect effect = new Effect();

        public override void UseAbility()
        {
            base.UseAbility();

            target.TakeDamage(effect.damage);
            if (effect.continuingDamageTurns > 0)
            {
                target.AddEffectOverTurn(effect);
            }         
        }
    }
}
