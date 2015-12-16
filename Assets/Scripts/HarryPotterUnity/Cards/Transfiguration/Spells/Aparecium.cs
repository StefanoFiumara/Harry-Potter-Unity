using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    [UsedImplicitly]
    public class Aparecium : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int handCount = Player.Hand.Cards.Count;

            var hand = Player.Hand.Cards.Select(card => card.gameObject).ToList();

            if (!Player.IsLocalPlayer)
            {
                var tween = new FlipCardsTween
                {
                    Targets = hand.ToList(),
                    Flip = FlipState.FaceUp,
                    TimeUntilNextTween = 1f
                };
                GameManager.TweenQueue.AddTweenToQueue(tween);
            }
              
            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.Hand.Cards[i];

                if (card.Type != Type.Lesson) continue;
                
                Player.InPlay.Add(card);

                hand.Remove(card.gameObject);
            }

            if (!Player.IsLocalPlayer)
            {
                var tween = new FlipCardsTween
                {
                    Targets = hand,
                    Flip = FlipState.FaceDown
                };
                GameManager.TweenQueue.AddTweenToQueue(tween);
            }
        }
    }
}