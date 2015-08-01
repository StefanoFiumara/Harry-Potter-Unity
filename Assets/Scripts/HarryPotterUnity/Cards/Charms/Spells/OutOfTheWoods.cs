using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    [UsedImplicitly]
    public class OutOfTheWoods : GenericSpell {
        protected override void SpellAction(List<GenericCard> targets)
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;

            var enemyHand = Player.OppositePlayer.Hand.Cards.Select(card => card.gameObject).ToList();

            if(Player.IsLocalPlayer) GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(new List<GameObject>(enemyHand), 
                                                                                               FlipStates.FaceUp, 
                                                                                               timeUntilNextTween: 1f));

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                if (card.CardType != CardTypes.Creature) continue;

                Player.OppositePlayer.Hand.Remove(card);
                Player.OppositePlayer.Discard.Add(card);

                enemyHand.Remove(card.gameObject);
            }

            if(Player.IsLocalPlayer) GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(enemyHand, FlipStates.FaceDown));
        }
    }
}
