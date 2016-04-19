﻿using System.Linq;
using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class SlytherinMatch : BaseMatch
    {
        protected override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            var cards = winner.Discard.GetHealableCards().Take(15);

            winner.Deck.AddAll(cards);
            winner.Deck.Shuffle();
        }
    }
}