using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

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
            var lesson = Player.Discard.GetCards(c => c.Type == Type.Lesson).First();

            Player.InPlay.Add(lesson);
            Player.Discard.RemoveAll(new[] {lesson});

            Player.UseActions();
        }

        public override void OnInPlayBeforeTurnAction() { }
        public override void OnInPlayAfterTurnAction() { }
        public override void OnEnterInPlayAction() { }
        public override void OnExitInPlayAction() { }
    }
}