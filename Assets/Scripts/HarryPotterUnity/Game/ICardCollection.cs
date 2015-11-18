using System.Collections.Generic;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;

namespace HarryPotterUnity.Game
{
    public interface ICardCollection
    {
        List<BaseCard> Cards { get; } 
        void Add(BaseCard card);
        void Remove(BaseCard card);

        void AddAll(IEnumerable<BaseCard> cards);
        void RemoveAll(IEnumerable<BaseCard> cards);
    }
}
