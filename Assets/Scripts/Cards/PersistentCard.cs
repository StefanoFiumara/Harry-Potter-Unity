public interface PersistentCard {

    void OnInPlayBeforeTurnAction();
    void OnInPlayAfterTurnAction();
    void OnSelectedAction();
    void OnEnterInPlayAction();
    void OnExitInPlayAction();
}
