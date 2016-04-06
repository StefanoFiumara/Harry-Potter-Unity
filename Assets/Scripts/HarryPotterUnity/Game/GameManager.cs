using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Tween;
using HarryPotterUnity.UI.Camera;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public static class GameManager {

        public const int PREVIEW_LAYER = 9;
        public const int CARD_LAYER = 10;
        public const int VALID_CHOICE_LAYER = 11;
        public const int IGNORE_RAYCAST_LAYER = 2;
        public const int DECK_LAYER = 12;

        public static byte _networkIdCounter;
        
        public static readonly List<BaseCard> AllCards = new List<BaseCard>(); 

        public static readonly PreviewCamera PreviewCamera = GameObject.Find("Preview Camera").GetComponent<PreviewCamera>();
        
        public static readonly TweenQueue TweenQueue = new TweenQueue();

        public static PhotonView Network;
        
        public static void DisableCards(List<BaseCard> cards)
        {
            cards.ForEach(card => card.Disable());
        }

        public static void EnableCards(List<BaseCard> cards)
        {
            cards.ForEach(card => card.Enable());
        }
    }
}
