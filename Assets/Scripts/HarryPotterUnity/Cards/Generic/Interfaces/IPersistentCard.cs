namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface IPersistentCard {

        void OnInPlayBeforeTurnAction();
        void OnInPlayAfterTurnAction();
        void OnSelectedAction(); //Might not need this
        void OnEnterInPlayAction();
        void OnExitInPlayAction();
    }
}
