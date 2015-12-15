using HarryPotterUnity.Game;
using UnityEngine;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class PracticeMatch : BaseMatch
    {
        protected override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
            winner.Deck.DrawCard();
        }
    }
}
