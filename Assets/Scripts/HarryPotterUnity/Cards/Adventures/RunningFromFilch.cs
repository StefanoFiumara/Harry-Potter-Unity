using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.PlayerConstraints;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class RunningFromFilch : AdventureWithConstraints
    {
        protected override void AddConstraints()
        {
            this._constraints.Add(new CannotPlayTypeConstraint(Type.Lesson));
        }

        protected override bool CanOpponentSolve()
        {
            return true;
        }

        protected override void Solve()
        {
            int handCount = this.Player.OppositePlayer.Hand.Cards.Count;

            for (int i = handCount - 1; i >= 0; i--)
            {
                var card = this.Player.OppositePlayer.Hand.Cards[i];

                this.Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            this.Player.OppositePlayer.Deck.DrawCard();
            this.Player.OppositePlayer.Deck.DrawCard();
            this.Player.OppositePlayer.Deck.DrawCard();
        }
    }
}