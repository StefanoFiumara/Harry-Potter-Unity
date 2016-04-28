using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Interfaces
{
    public interface IPersistentCard
    {
        void OnInPlayBeforeTurnAction();
        void OnInPlayAfterTurnAction();

        bool CanPerformInPlayAction();
        //TODO: Default behavior for OnSelectedAction to preview the card before activating effect.
        void OnInPlayAction(List<BaseCard> targets = null); 

        void OnEnterInPlayAction();
        void OnExitInPlayAction();
    }
}
