namespace HarryPotterUnity.Cards.Adventures
{
    public class TrollInTheBathroom : BaseAdventure
    {
        public override void OnInPlayBeforeTurnAction()
        {
            this.Player.OppositePlayer.TakeDamage(this, 2);
        }

        protected override bool CanOpponentSolve()
        {
            return this.Player.OppositePlayer.Hand.Cards.Count >= 7;
        }

        protected override void Solve()
        {
            int amountToDiscard = 7;

            for (int i = amountToDiscard; i > 0; i--)
            {
                var card = this.Player.OppositePlayer.Hand.GetRandomCard();

                this.Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            this.Player.TakeDamage(this, 4);
        }
    }
}