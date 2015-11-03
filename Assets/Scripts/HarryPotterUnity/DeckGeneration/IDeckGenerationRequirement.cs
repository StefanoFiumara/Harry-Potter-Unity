using System.Collections.Generic;
using HarryPotterUnity.Cards;

namespace HarryPotterUnity.DeckGeneration
{
    public interface IDeckGenerationRequirement
    {
        bool MeetsRequirement(List<BaseCard> currentDeck);
    }
}
