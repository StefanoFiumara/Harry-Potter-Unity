using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class OutOfTheWoods : BaseSpell {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;

            var enemyHand = Player.OppositePlayer.Hand.Cards.Select(card => card.gameObject).ToList();

            if(Player.IsLocalPlayer) GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(new List<GameObject>(enemyHand), 
                                                                                               FlipState.FaceUp, 
                                                                                               timeUntilNextTween: 1f));

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                if (card.Type != Type.Creature) continue;
                
                Player.OppositePlayer.Discard.Add(card);

                enemyHand.Remove(card.gameObject);
            }

            if(Player.IsLocalPlayer) GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(enemyHand, FlipState.FaceDown));
        }
    }
}
