using UnityEngine;
using Randero.Attribute;

namespace Randero.Core
{
    public class GameSession : MonoBehaviour
    {
        private TurnManager[] players = null;
        private int currentPlayerIndex = 0;
        bool firstSwitch = true;

        private void Start()
        {
            players = FindObjectsOfType<TurnManager>();
            foreach (var player in players)
            {
                player.InitialiseAbilityBar();
                player.StartTurnLogic();
            }

            if (currentPlayerIndex + 1 > 1)
            {
                players[0].EndTurnLogic();
            }
            else
            {
                players[1].EndTurnLogic();
            }

            Health.onApplyFreeze += SetMyFrozenState;
        }
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchTurn();
            }
        }

        private void SetMyFrozenState(bool isFrozen)
        {
            players[currentPlayerIndex].GetComponent<TurnManager>().SetIsFrozenState(isFrozen);
        }

        private void SwitchTurn()
        {
            players[currentPlayerIndex].EndTurnLogic();
            currentPlayerIndex += 1;
            if (currentPlayerIndex > 1)
            {
                currentPlayerIndex = 0;
            }
            Debug.Log("Now it is " + (currentPlayerIndex+1) + " Player's turn");

            players[currentPlayerIndex].GetComponent<Health>().ApplyContinuingEffect();

            if (players[currentPlayerIndex].GetIsFrozen())
            {
                firstSwitch = false;
                SwitchTurn();                
                return;
            }

            if (!firstSwitch)
            {
                players[currentPlayerIndex].StartTurnLogic();
            }
            else
            {
                players[currentPlayerIndex].EnableAbilities();
            }
            firstSwitch = false;
        }
    }
}
