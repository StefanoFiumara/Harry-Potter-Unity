namespace HarryPotterUnity.Cards.Adventures
{
    public class TrollInTheBathroom : BaseAdventure
    {
        public override void OnInPlayBeforeTurnAction()
        {
            Player.OppositePlayer.TakeDamage(this, 2);
        }

        protected override bool CanOpponentSolve()
        {
            return Player.OppositePlayer.Hand.Cards.Count >= 7;
        }

        protected override void Solve()
        {
            int amountToDiscard = 7;

            for (int i = amountToDiscard; i > 0; i--)
            {
                var card = Player.OppositePlayer.Hand.GetRandomCard();

                Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            Player.TakeDamage(this, 4);
        }
    }
}