using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class UnusualPets : BaseAdventure
    {
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.OppositePlayer.InPlay.Creatures.Any();
        }

        public override void OnInPlayBeforeTurnAction()
        {
            Player.OppositePlayer.TakeDamage(this, 4);
        }

        protected override bool CanOpponentSolve()
        {
            return Player.OppositePlayer.InPlay.Creatures.Count >= 2;
        }

        protected override void Solve()
        {
            int amountToDiscard = 2;

            for (int i = 0; i < amountToDiscard; i++)
            {
                var card = Player.OppositePlayer.InPlay.GetRandomCard(ofType:Type.Creature);
                
                Player.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            Player.OppositePlayer.Deck.DrawCard();
        }
    }
}