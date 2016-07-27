using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Adventures
{
    public class RidingTheCentaur : BaseAdventure
    {
        public override void OnInPlayAfterTurnAction()
        {
            Player.TypeImmunity.Add(Type.Creature);
            Player.OnNextTurnStartEvent += () => Player.TypeImmunity.Remove(Type.Creature);
        }

        protected override bool CanOpponentSolve()
        {
            return Player.OppositePlayer.InPlay.Cards.Count >= 4;

        }

        protected override void Solve()
        {
            int amountToReturn = 4;

            for (int i = amountToReturn; i > 0; i--)
            {
                var card = Player.OppositePlayer.InPlay.GetRandomCard();

                Player.OppositePlayer.Hand.Add(card);
            }
        }

        protected override void Reward()
        {
            Player.OppositePlayer.Deck.DrawCard();
        }
    }
}