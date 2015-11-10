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

        public virtual bool CanPerformInPlayAction()
        {
            return false;
        }
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnSelectedAction() { }
        public virtual void OnEnterInPlayAction() { }
        public virtual void OnExitInPlayAction() { }
    }
}