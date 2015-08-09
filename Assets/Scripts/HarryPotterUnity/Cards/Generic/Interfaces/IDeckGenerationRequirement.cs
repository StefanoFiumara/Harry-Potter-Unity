using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface IDeckGenerationRequirement
    {
        bool MeetsRequirement(List<GenericCard> currentDeck);
    }
}
