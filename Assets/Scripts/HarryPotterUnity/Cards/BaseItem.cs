using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseItem : BaseCard, IPersistentCard
    {
        protected override Type GetCardType()
        {
            return Type.Item;
        }

        public abstract void OnInPlayBeforeTurnAction();
        public abstract void OnInPlayAfterTurnAction();
        public abstract bool CanPerformInPlayAction();
        public abstract void OnSelectedAction();
        public abstract void OnEnterInPlayAction();
        public abstract void OnExitInPlayAction();
    }
}