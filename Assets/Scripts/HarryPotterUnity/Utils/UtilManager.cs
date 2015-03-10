using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public static class UtilManager {

        public const int PreviewLayer = 9;
        public const int CardLayer = 10;
        public const int ValidChoiceLayer = 11;
        public const int IgnoreRaycastLayer = 2;
        public const int DeckLayer = 12;

        public static byte NetworkIdCounter;
        public static readonly List<GenericCard> AllCards = new List<GenericCard>(); 

        public static Camera PreviewCamera;
        public static readonly Vector3 DefaultPreviewCameraPos = new Vector3(-400, 255, -70);
        
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
