using System.Collections.Generic;

namespace HarryPotterUnity.Cards.DeckGenerationRequirements
{
    public interface IDeckGenerationRequirement
    {
        bool MeetsRequirement(List<BaseCard> currentDeck);
    }
}
