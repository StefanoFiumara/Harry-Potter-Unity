using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Spells
{
    public class AntiCheatingSpell : BaseSpell
    {
        //TODO: Test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            var enemyHand = Player.OppositePlayer.Hand.Cards.Select(card => card.gameObject).ToList();

            if (Player.IsLocalPlayer)
            {
                var tween = new FlipCardsTween
                {
                    Targets = enemyHand.ToList(),
                    Flip = FlipState.FaceUp,
                    TimeUntilNextTween = 1f
                };
                GameManager.TweenQueue.AddTweenToQueue(tween);
            }

            var lesson = Player.OppositePlayer.Hand.Lessons.FirstOrDefault();

            if (lesson != null)
            {
                Player.OppositePlayer.Discard.Add(lesson);
                enemyHand.Remove(lesson.gameObject);
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