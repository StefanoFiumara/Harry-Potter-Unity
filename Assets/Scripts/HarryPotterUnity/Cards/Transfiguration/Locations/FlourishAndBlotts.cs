using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Locations
{
    public class FlourishAndBlotts : BaseLocation
    {
        public override bool CanPerformInPlayAction()
        {
            var player = Player.IsLocalPlayer ? Player : Player.OppositePlayer;

            return player.CanUseActions() &&
                   player.InPlay.Lessons.Count >= 2;
        }

        public override void OnSelectedAction(List<BaseCard> targets = null)
        {
            //HACK: Need a nicer way to determine which player is activating this effect
            var player = Player.CanUseActions() ? Player : Player.OppositePlayer;

            var lessons = player.InPlay.Lessons.Take(2);

            player.Discard.AddAll(lessons);

            for (int i = 0; i < 5; i++)
            {
                player.Deck.DrawCard();
            }

            player.UseActions();
        }
    }
}