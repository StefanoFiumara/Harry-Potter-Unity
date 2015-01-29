namespace Assets.Scripts.Cards
{
    public interface IPersistentCard {

        void OnInPlayBeforeTurnAction();
        void OnInPlayAfterTurnAction();
        void OnSelectedAction();
        void OnEnterInPlayAction();
        void OnExitInPlayAction();
    }
}
