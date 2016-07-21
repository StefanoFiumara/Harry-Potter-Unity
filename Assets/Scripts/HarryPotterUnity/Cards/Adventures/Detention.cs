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
            _constraints.Add(new CannotPlayTypeConstraint(Type.Adventure));
        }

        protected override bool CanOpponentSolve()
        {
            return Player.OppositePlayer.Hand.Adventures.Count >= 2;
        }

        protected override void Solve()
        {
            var adventures = Player.OppositePlayer.Hand.Adventures.Take(2);

            Player.OppositePlayer.Discard.AddAll(adventures);
        }

        protected override void Reward()
        {
            Player.OppositePlayer.Deck.DrawCard();
        }
    }
}