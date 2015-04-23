namespace HarryPotterUnity.Cards.Interfaces
{
    public interface ICardPlayRequirement
    {
        bool MeetsRequirement();

        void OnRequirementMet();
    }
}
