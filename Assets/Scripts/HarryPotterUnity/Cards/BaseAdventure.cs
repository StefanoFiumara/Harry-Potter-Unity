using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseAdventure : BaseCard, IPersistentCard
    {
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }

        protected abstract bool CanOpponentSolve();
        protected abstract void Solve();
        protected abstract void Reward();

        public virtual void OnEnterInPlayAction() { }
        public virtual void OnExitInPlayAction() { }

        public bool CanPerformInPlayAction()
        {
            bool isOppositePlayer = this.Player.IsLocalPlayer == false;

            return isOppositePlayer 
                && this.Player.OppositePlayer.CanUseActions() 
                && this.CanOpponentSolve();
        }

        public void OnInPlayAction(List<BaseCard> targets = null)
        {
            this.Player.Discard.Add(this);
            this.Solve();
            this.Reward();
        }
        
        protected override Type GetCardType()
        {
            return Type.Adventure;
        }
    }
}