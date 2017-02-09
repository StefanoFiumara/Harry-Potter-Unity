namespace HarryPotterUnity.Cards.Adventures
{
    public class FivePointsFromGryffindor : BaseAdventure
    {
        public override void OnInPlayAfterTurnAction()
        {
            this.Player.OppositePlayer.OnNextTurnStartEvent += () => this.Player.OppositePlayer.AddActions(-1);
        }

        protected override bool CanOpponentSolve()
        {
            return this.Player.OppositePlayer.Hand.Cards.Count >= 5;
        }

        protected override void Solve()
        {
            int amountToDiscard = 5;

            for (int i = amountToDiscard; i > 0; i--)
            {
                var card = this.Player.OppositePlayer.Hand.GetRandomCard();

                this.Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            this.Player.TakeDamage(this, 5);
        }
    }
}