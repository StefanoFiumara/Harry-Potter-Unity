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

        public abstract void OnInPlayBeforeTurnAction();
        public abstract void OnInPlayAfterTurnAction();
        public abstract bool CanPerformInPlayAction();
        public abstract void OnSelectedAction();
        public abstract void OnEnterInPlayAction();
        public abstract void OnExitInPlayAction();
    }
}