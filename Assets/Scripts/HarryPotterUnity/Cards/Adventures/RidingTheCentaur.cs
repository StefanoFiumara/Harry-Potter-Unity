using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class RidingTheCentaur : BaseAdventure
    {
        public override void OnInPlayAfterTurnAction()
        {
            this.Player.TypeImmunity.Add(Type.Creature);
            this.Player.OnNextTurnStartEvent += () => this.Player.TypeImmunity.Remove(Type.Creature);
        }

        protected override bool CanOpponentSolve()
        {
            return this.Player.OppositePlayer.InPlay.Cards.Count >= 4;

        }

        protected override void Solve()
        {
            int amountToReturn = 4;

            for (int i = amountToReturn; i > 0; i--)
            {
                var card = this.Player.OppositePlayer.InPlay.GetRandomCard();

                this.Player.OppositePlayer.Hand.Add(card);
            }
        }

        protected override void Reward()
        {
            this.Player.OppositePlayer.Deck.DrawCard();
        }
    }
}