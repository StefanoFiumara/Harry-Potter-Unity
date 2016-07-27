using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.PlayerConstraints;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class RunningFromFilch : AdventureWithConstraints
    {
        protected override void AddConstraints()
        {
            _constraints.Add(new CannotPlayTypeConstraint(Type.Lesson));
        }

        protected override bool CanOpponentSolve()
        {
            return true;
        }

        protected override void Solve()
        {
            int handCount = Player.OppositePlayer.Hand.Cards.Count;

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = Player.OppositePlayer.Hand.Cards[i];

                Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            Player.OppositePlayer.Deck.DrawCard();
            Player.OppositePlayer.Deck.DrawCard();
            Player.OppositePlayer.Deck.DrawCard();
        }
    }
}