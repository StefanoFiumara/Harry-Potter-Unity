using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Quidditch.Matches
{
    public class GryffindorMatch : BaseMatch
    {
        public override void OnPlayerHasWonMatch(Player winner, Player loser)
        {
            winner.OnNextTurnStartEvent += () => winner.AddActions(2);
        }
    }
}