using System.Collections.Generic;
using UnityEngine;
using Randero.Combat;
using Randero.UI;
// TODO if two players have same character to start with, then player 1's ability will have its caster and target swapped
namespace Randero.Core
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] AbilityBarUI abilityBar = null;
        [SerializeField] CharacterClass[] characterClasses = null;

        CharacterClass[] clonedCharacters;
        int currentCharacterClassIndex = 0;
        bool isFrozen = false;
        AbilityStore store = null;
        GameObject opponent = null;

        private void Awake()
        {
            store = GetComponent<AbilityStore>();
            CloneCharacters();
        }

        private void Start()
        {
            GetOpponent();
        }

        private void CloneCharacters()
        {
            var temp = new List<CharacterClass>();
            foreach (var characterClass in characterClasses)
            {
                var copy = Instantiate(characterClass);
                copy.CloneAbilities();
                temp.Add(copy);
                
            }
            clonedCharacters = temp.ToArray();
        }

        public void InitialiseAbilityBar()
        {
            if (abilityBar != null)
            {
                abilityBar.Initialise(store);
            }
        }

        public void StartTurnLogic()
        {
            store.ClearStore();

            // Calculate a random character class index 
            currentCharacterClassIndex = GetDifferentCharacterClassIndex();

            // Set current character sprite
            GetComponentInChildren<SpriteRenderer>().sprite = clonedCharacters[currentCharacterClassIndex].GetCharacterSprite();

            // Get the abilities from that character class, then display them in card bar
            Ability[] abilities = clonedCharacters[currentCharacterClassIndex].GetAbilities();
            Debug.Log(abilities.Length + " abilities for " + gameObject.name);
            if (abilities == null)
            {
                Debug.LogError("WTF abilities array is null?!");
            }
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i].AssignCasterAndTarget(gameObject, opponent);
                store.AddToStore(abilities[i], i);
            }

            // enable ability bar access
            EnableAbilities();
        }

        public void EndTurnLogic()
        {
            // disable ability bar access
            CanvasGroup[] canvasGroups = abilityBar.GetComponentsInChildren<CanvasGroup>();
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.enabled = false;
            }
        }

        public void EnableAbilities()
        {
            CanvasGroup[] canvasGroups = abilityBar.GetComponentsInChildren<CanvasGroup>();
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.enabled = true;
            }
        }

        public void SetIsFrozenState(bool isFrozen)
        {
            this.isFrozen = isFrozen;
        }

        public bool GetIsFrozen()
        {
            return isFrozen;
        }

        private int GetDifferentCharacterClassIndex()
        {
            int nextCharacterClassIndex = Random.Range(0, characterClasses.Length);
            while (currentCharacterClassIndex == nextCharacterClassIndex)
            {
                nextCharacterClassIndex = Random.Range(0, characterClasses.Length);
            }
            return nextCharacterClassIndex;
        }

        public GameObject GetOpponent()
        {
            TurnManager[] players = FindObjectsOfType<TurnManager>();
            if (players[0].gameObject.Equals(gameObject))
            {
                opponent = players[1].gameObject;
            }
            else
            {
                opponent = players[0].gameObject;
            }

            return opponent;
        }
    }
}
