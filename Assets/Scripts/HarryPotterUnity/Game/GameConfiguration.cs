using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;

namespace HarryPotterUnity.Game
{
    public class GameConfiguration : MonoBehaviour
    {
        [Header("Game Options")]
        [SerializeField, UsedImplicitly]
        private bool _enableDebugMode;

        [Header("Add pre-determined cards to each player's deck")]
        [SerializeField, UsedImplicitly]
        private List<GameObject> _player1Deck = new List<GameObject>();
        [SerializeField, UsedImplicitly]
        private List<GameObject> _player2Deck = new List<GameObject>();

        [Header("Select Default Starting Characters")]
        [SerializeField, UsedImplicitly]
        private GameObject _player1StartingCharacter;
        [SerializeField, UsedImplicitly]
        private GameObject _player2StartingCharacter;

        private void Awake()
        {
            GameManager.DebugModeEnabled = _enableDebugMode;
            if (_enableDebugMode)
            {
                SetDebugModeConfiguration();
            }
            
        }

        private void SetDebugModeConfiguration()
        {
            if (_player1Deck.Any(o => o.GetComponent<BaseCard>() == null)
                || _player2Deck.Any(o => o.GetComponent<BaseCard>() == null))
            {
                Log.Error("Incorrect Configuration for Player Test Decks in Game Manager (one or more objects is not a Card).");
            }

            if (_player1StartingCharacter.GetComponent<BaseCard>() is BaseCharacter == false
                || _player2StartingCharacter.GetComponent<BaseCard>() is BaseCharacter == false)
            {
                Log.Error("Incorrect Configuration for Player Test Decks in Game Manager (The assigned starting characters do not derive from BaseCharacter)");
            }

            GameManager.Debug_Player1Deck = _player1Deck;
            GameManager.Debug_Player2Deck = _player2Deck;
            GameManager.Debug_Player1StartingCharacter = _player1StartingCharacter;
            GameManager.Debug_Player2StartingCharacter = _player2StartingCharacter;
        }
    }
}