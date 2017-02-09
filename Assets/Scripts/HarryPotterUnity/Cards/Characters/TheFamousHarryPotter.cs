namespace HarryPotterUnity.Cards.Characters
{
    public class TheFamousHarryPotter : BaseCharacter
    {
        public override void OnInPlayBeforeTurnAction()
        {
            if (this.Player.Hand.Cards.Count <= 4)
            {
                this.Player.Deck.DrawCard();
            }
        }
    }
}
