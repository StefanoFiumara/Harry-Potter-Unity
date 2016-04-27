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

        [Header("Debug Mode Settings")]
        [Header("Add pre-determined cards to each player's deck")]
        [SerializeField, UsedImplicitly]
        private List<GameObject> _player1Deck = new List<GameObject>();
        [SerializeField, UsedImplicitly]
        private List<GameObject> _player2Deck = new List<GameObject>();

        private void Awake()
        {
            if (_player1Deck.Any(o => o.GetComponent<BaseCard>() == null)
                || _player2Deck.Any(o => o.GetComponent<BaseCard>() == null))
            {
                Log.Error("Incorrect Configuration for Player Test Decks in Game Manager (one or more objects is not a Card).");
            }

            GameManager.DebugModeEnabled = _enableDebugMode;
            GameManager.Player1Deck = _player1Deck;
            GameManager.Player2Deck = _player2Deck;
        }
    }
}