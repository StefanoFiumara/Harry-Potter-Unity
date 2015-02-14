namespace HarryPotterUnity.Cards
{
    public interface IPersistentCard {

        void OnInPlayBeforeTurnAction();
        void OnInPlayAfterTurnAction();
        void OnSelectedAction();
        void OnEnterInPlayAction();
        void OnExitInPlayAction();
    }
}
