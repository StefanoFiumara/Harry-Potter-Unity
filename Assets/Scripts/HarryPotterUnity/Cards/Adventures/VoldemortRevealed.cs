using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Cards.PlayerConstraints;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class VoldemortRevealed : AdventureWithConstraints
    {
        protected override void AddConstraints()
        {
            _constraints.Add(new CannotPlayTypeConstraint(Type.Spell));
        }

        protected override bool CanOpponentSolve()
        {
            return true;
        }

        protected override void Solve()
        {
            Player.OppositePlayer.TakeDamage(this, 7);
        }

        protected override void Reward()
        {
            var cards = Player.OppositePlayer.Discard.NonHealingCards.Take(2);

            Player.OppositePlayer.Deck.AddAll(cards);
        }
    }
}