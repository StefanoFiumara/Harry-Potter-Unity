using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public class GenericCreature : GenericCard, IPersistentCard {

        [SerializeField, UsedImplicitly]
        private int _damagePerTurn;

        [SerializeField, UsedImplicitly]
        private int _health;

        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }

        public void OnEnterInPlayAction()
        {
            Player.CreaturesInPlay++;
            Player.DamagePerTurn += _damagePerTurn;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.CreaturesInPlay--;
            Player.DamagePerTurn -= _damagePerTurn;
        }

        //Generic Creatures don't do anything special on these actions
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
  
    }
}
