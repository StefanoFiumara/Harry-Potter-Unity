public class GenericCreature : GenericCard, IPersistentCard {

    public Lesson.LessonTypes CostType;

    public int CostAmount;

    public int DamagePerTurn;
    public int Health;

    public void OnMouseUp()
    {
        if (State != CardStates.InHand) return;

        if (!_Player.CanUseAction()) return;

        if (_Player.AmountLessonsInPlay < CostAmount || !_Player.LessonTypesInPlay.Contains(CostType)) return;

        if (!MeetsAdditionalRequirements()) return;

        _Player._Hand.Remove(this);
        _Player._InPlay.Add(this);
        _Player.UseAction();
    }

    public void OnEnterInPlayAction()
    {
        _Player.CreaturesInPlay++;
        _Player.DamagePerTurn += DamagePerTurn;

        State = CardStates.InPlay;
    }

    public void OnExitInPlayAction()
    {
        _Player.CreaturesInPlay--;
        _Player.DamagePerTurn -= DamagePerTurn;
    }

    protected virtual bool MeetsAdditionalRequirements()
    {
        return true;
    }

    //Generic Creatures don't do anything special on these actions
    public void OnInPlayBeforeTurnAction() { }
    public void OnInPlayAfterTurnAction() { }
    public void OnSelectedAction() { }
  
}
