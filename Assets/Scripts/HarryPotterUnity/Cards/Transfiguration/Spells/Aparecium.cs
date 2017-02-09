using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;

namespace HarryPotterUnity.Cards.Transfiguration.Spells
{
    public class Aparecium : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int handCount = this.Player.Hand.Cards.Count;

            var hand = this.Player.Hand.Cards.Select(card => card.gameObject).ToList();

            if (!this.Player.IsLocalPlayer)
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
                var card = this.Player.Hand.Cards[i];

                if (card.Type != Type.Lesson) continue;

                this.Player.InPlay.Add(card);

                hand.Remove(card.gameObject);
            }

            if (!this.Player.IsLocalPlayer)
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