namespace HarryPotterUnity.Cards.Adventures
{
    public class FivePointsFromGryffindor : BaseAdventure
    {
        public override void OnInPlayAfterTurnAction()
        {
            Player.OppositePlayer.OnNextTurnStart += () => Player.OppositePlayer.AddActions(-1);
        }

        protected override bool CanOpponentSolve()
        {
            return Player.OppositePlayer.Hand.Cards.Count >= 5;
        }

        protected override void Solve()
        {
            int amountToDiscard = 5;

            for (int i = amountToDiscard; i > 0; i--)
            {
                var card = Player.OppositePlayer.Hand.GetRandomCard();

                Player.OppositePlayer.Discard.Add(card);
            }
        }

        protected override void Reward()
        {
            Player.TakeDamage(this, 5);
        }
    }
}