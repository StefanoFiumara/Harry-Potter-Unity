using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class Remembrall : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && 
                   Player.Discard.CountCards(c => c.Type == Type.Lesson) > 0;
        }

        public override void OnSelectedAction()
        {
            var lesson = Player.Discard.Cards.First(c => c.Type == Type.Lesson);

            Player.InPlay.Add(lesson);
            
            Player.UseActions();
        }
    }
}