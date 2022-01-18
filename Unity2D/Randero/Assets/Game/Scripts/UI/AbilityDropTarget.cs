using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Randero.Combat;
using Randero.Utils;

namespace Randero.UI
{
    /// <summary>
    /// Handles using ability when ability dropped into the world.
    /// 
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space. 
    /// </summary>
    public class AbilityDropTarget : MonoBehaviour, IDragDestination<Ability>
    {
        public void AddItem(Ability ability)
        {
            ability.UseAbility();
        }
    }
}
