using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards
{
    [UsedImplicitly]
    public abstract class BaseCharacter : BaseCard, IPersistentCard
    {
        protected sealed override Type GetCardType()
        {
            return Type.Character;
        }

        public virtual bool CanPerformInPlayAction()
        {
            return false;
        }

        public virtual void OnSelectedAction(List<BaseCard> targets = null) { }
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnEnterInPlayAction() { }
        public virtual void OnExitInPlayAction() { }
    }
}