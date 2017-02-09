using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class Remembrall : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() 
                && this.Player.Discard.Lessons.Count > 0
                && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var lesson = this.Player.Discard.Lessons.First();

            this.Player.InPlay.Add(lesson);

            this.Player.UseActions();
        }
    }
}