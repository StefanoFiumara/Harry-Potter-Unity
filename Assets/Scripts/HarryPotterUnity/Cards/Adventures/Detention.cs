using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.PlayerConstraints;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class Detention : AdventureWithConstraints
    {
        protected override void AddConstraints()
        {
            this._constraints.Add(new CannotPlayTypeConstraint(Type.Adventure));
        }

        protected override bool CanOpponentSolve()
        {
            return this.Player.OppositePlayer.Hand.Adventures.Count >= 2;
        }

        protected override void Solve()
        {
            var adventures = this.Player.OppositePlayer.Hand.Adventures.Take(2);

            this.Player.OppositePlayer.Discard.AddAll(adventures);
        }

        protected override void Reward()
        {
            this.Player.OppositePlayer.Deck.DrawCard();
        }
    }
}