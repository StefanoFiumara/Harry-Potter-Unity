using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Tween;
using HarryPotterUnity.UI.Camera;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public class DebugModeOptions
    {
        public List<GameObject> Player1Deck { get; set; }
        public List<GameObject> Player2Deck { get; set; }
        public GameObject Player1StartingCharacter { get; set; }
        public GameObject Player2StartingCharacter { get; set; }
    }

    public static class GameManager
    {
        public const int PREVIEW_LAYER = 9;
        public const int CARD_LAYER = 10;
        public const int VALID_CHOICE_LAYER = 11;
        public const int IGNORE_RAYCAST_LAYER = 2;
        public const int DECK_LAYER = 12;

        public static bool IsInputGathererActive { get; set; }

        public static byte NetworkIdCounter { get; set; }

        public static readonly List<BaseCard> AllCards = new List<BaseCard>();

        public static readonly PreviewCamera PreviewCamera = GameObject.Find("Preview Camera").GetComponent<PreviewCamera>();

        public static readonly TweenQueue TweenQueue = new TweenQueue();

        public static PhotonView Network { get; set; }

        public static void DisableCards(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards) card.Disable();
        }

        public static bool DebugModeEnabled { get; set; }

        public static DebugModeOptions DebugModeOptions { get; set; }

        public static void EnableCards(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards) card.Enable();
        }

        public static List<BaseCard> GetPlayerTestDeck(int playerId)
        {
            var prefabList = playerId == 0
                ? DebugModeOptions.Player1Deck
                : DebugModeOptions.Player2Deck;

            return prefabList.Select(o => o.GetComponent<BaseCard>()).ToList();
        }

        public static BaseCard GetPlayerTestCharacter(int playerId)
        {
            var obj = playerId == 0
                ? DebugModeOptions.Player1StartingCharacter
                : DebugModeOptions.Player2StartingCharacter;

            return obj.GetComponent<BaseCard>();
        }
    }
}
