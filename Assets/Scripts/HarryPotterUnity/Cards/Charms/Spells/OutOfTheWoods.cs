using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class OutOfTheWoods : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;

            var enemyHand = Player.OppositePlayer.Hand.Cards.Select(card => card.gameObject).ToList();

            if(Player.IsLocalPlayer)
            {
                var tween = new FlipCardsTween
                {
                    Targets = enemyHand.ToList(),
                    Flip = FlipState.FaceUp,
                    TimeUntilNextTween = 1f
                };
                GameManager.TweenQueue.AddTweenToQueue( tween );
            }

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                if (card.Type != Type.Creature) continue;
                
                Player.OppositePlayer.Discard.Add(card);

                enemyHand.Remove(card.gameObject);
            }

            if (Player.IsLocalPlayer)
            {
                var tween = new FlipCardsTween
                {
                    Targets = enemyHand,
                    Flip = FlipState.FaceDown
                };
                GameManager.TweenQueue.AddTweenToQueue(tween);
            }
        }
    }
}
