namespace HarryPotterUnity.Cards.PlayerConstraints
{
    public interface IPlayerConstraint
    {
        bool MeetsConstraint(BaseCard cardToPlay);
    }
}