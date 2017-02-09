using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards
{
    public class BaseLocation : BaseCard, IPersistentCard
    {
        protected sealed override Type GetCardType()
        {
            return Type.Location;
        }
        
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }

        public virtual bool CanPerformInPlayAction() { return false; }
        public virtual void OnInPlayAction(List<BaseCard> targets = null) { }

        public virtual void OnEnterInPlayAction()
        {
            BaseCard existingLocation = this.Player.InPlay.Cards.Concat(this.Player.OppositePlayer.InPlay.Cards)
                    .SingleOrDefault(c => c.Type == Type.Location && c != this);

            if (existingLocation != null)
            {
                existingLocation.Player.Discard.Add(existingLocation);
            }
        }
        public virtual void OnExitInPlayAction() { }
    }
}