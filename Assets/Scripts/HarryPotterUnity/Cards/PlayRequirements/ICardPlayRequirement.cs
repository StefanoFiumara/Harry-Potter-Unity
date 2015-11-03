namespace HarryPotterUnity.Cards.PlayRequirements
{
    public interface ICardPlayRequirement
    {
        bool MeetsRequirement();

        void OnRequirementMet();
    }
}
