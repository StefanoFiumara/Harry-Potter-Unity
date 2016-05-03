using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class HufflepuffMatch : BaseMatch
    {
        public override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            for (int i = 0; i < 5; i++)
            {
                winner.Deck.DrawCard();
            }

            loser.TakeDamage(this, 5);
        }
    }
}