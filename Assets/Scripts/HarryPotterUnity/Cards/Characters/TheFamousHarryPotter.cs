namespace HarryPotterUnity.Cards.Characters
{
    public class TheFamousHarryPotter : BaseCharacter
    {
        public override void OnInPlayBeforeTurnAction()
        {
            if (Player.Hand.Cards.Count <= 4)
            {
                Player.Deck.DrawCard();
            }
        }
    }
}
