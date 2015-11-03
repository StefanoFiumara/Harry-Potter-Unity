using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    [UsedImplicitly]
    public class Remembrall : BaseCard, IPersistentCard
    {
        protected override void OnClickAction(List<BaseCard> targets)
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