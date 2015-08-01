namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface ICardPlayRequirement
    {
        bool MeetsRequirement();

        void OnRequirementMet();
    }
}
