using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards
{
    public class BaseLocation : BaseCard, IPersistentCard
    {
        protected override Type GetCardType()
        {
            return Type.Location;
        }

        protected override void OnClickAction(List<BaseCard> targets)
        {
            base.OnClickAction(targets);
            
            BaseCard existingLocation =
                Player.InPlay.Cards.Concat(Player.OppositePlayer.InPlay.Cards)
                    .SingleOrDefault(c => c.Type == Type.Location && c != this);

            if (existingLocation != null)
            {
                existingLocation.Player.Discard.Add(existingLocation);
            }
        }

        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }

        public virtual bool CanPerformInPlayAction() { return false; }
        public virtual void OnSelectedAction() { }

        public virtual void OnEnterInPlayAction() { }
        public virtual void OnExitInPlayAction() { }
    }
}