using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Aparecium : GenericSpell
    {
        protected override void SpellAction(List<GenericCard> targets)
        {
            int handCount = Player.Hand.Cards.Count;

            var hand = Player.Hand.Cards.Select(card => card.gameObject).ToList();

            if (!Player.IsLocalPlayer)
                GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(new List<GameObject>(hand),
                                                                          FlipStates.FaceUp,
                                                                          timeUntilNextTween: 1f));

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.Hand.Cards[i];

                if (card.Type != CardType.Lesson) continue;

                Player.Hand.Remove(card);
                Player.InPlay.Add(card);

                hand.Remove(card.gameObject);
            }

            if (!Player.IsLocalPlayer) GameManager.TweenQueue.AddTweenToQueue(new FlipCardsTween(hand, FlipStates.FaceDown));
        }
    }
}