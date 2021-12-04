using UnityEngine;
using Randero.Attribute;

namespace Randero.Combat
{
    public class Ability : ScriptableObject
    {
        [SerializeField] Sprite cardSprite = null;
        [SerializeField] ParticleSystem abilityParticles = null;

        protected Health caster = null;
        protected Health target = null;

        public Sprite GetCardSprite()
        {
            return cardSprite;
        }

        public virtual void UseAbility()
        {
            Debug.Log($"{caster.gameObject.name} is using {name} on {target.gameObject.name}");
            // damage dealing happens
        }

        public void AssignCasterAndTarget(GameObject caster, GameObject target)
        {
            this.caster = caster.GetComponent<Health>();
            this.target = target.GetComponent<Health>();
        }
    }
}
