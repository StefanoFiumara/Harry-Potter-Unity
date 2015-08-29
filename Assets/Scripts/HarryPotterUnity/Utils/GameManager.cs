using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public static class GameManager {

        public const int PREVIEW_LAYER = 9;
        public const int CARD_LAYER = 10;
        public const int VALID_CHOICE_LAYER = 11;
        public const int IGNORE_RAYCAST_LAYER = 2;
        public const int DECK_LAYER = 12;

        public static byte _networkIdCounter;

        public static readonly List<GenericCard> AllCards = new List<GenericCard>(); 

        public static Camera _previewCamera;
        
        public static readonly TweenQueue TweenQueue = new TweenQueue();

        public static void DisableCards(List<GenericCard> cards)
        {
            cards.ForEach(card => card.Disable());
        }

        public static void EnableCards(List<GenericCard> cards)
        {
            cards.ForEach(card => card.Enable());
        }
    }
}
