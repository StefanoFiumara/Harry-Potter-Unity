using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class Remembrall : GenericCard, IPersistentCard
    {
        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }
        
        public bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && 
                   Player.Discard.CountCards(c => c.Type == Type.Lesson) > 0;
        }

        public void OnSelectedAction()
        {
            var lesson = Player.Discard.GetCards(c => c.Type == Type.Lesson).First();

            Player.InPlay.Add(lesson);
            Player.Discard.RemoveAll(new[] {lesson});

            Player.UseActions();
        }

        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnEnterInPlayAction() { }
        public void OnExitInPlayAction() { }
    }
}