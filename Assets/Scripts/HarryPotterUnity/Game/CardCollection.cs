using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    public abstract class CardCollection : MonoBehaviour
    {
        public List<BaseCard> Cards { get; protected set; }

        #region CardType Shortcuts
        public List<BaseCard> Lessons
        {
            get { return Cards.Where(c => c.Type == Type.Lesson).ToList(); }
        }

        public List<BaseCard> Items
        {
            get { return Cards.Where(c => c.Type == Type.Item).ToList(); }
        }

        public List<BaseCard> Creatures
        {
            get { return Cards.Where(c => c.Type == Type.Creature).ToList(); }
        }

        public List<BaseCard> Spells
        {
            get { return Cards.Where(c => c.Type == Type.Spell).ToList(); }
        }

        public List<BaseCard> Locations
        {
            get { return Cards.Where(c => c.Type == Type.Location).ToList(); }
        }

        public List<BaseCard> Matches
        {
            get { return Cards.Where(c => c.Type == Type.Match).ToList(); }
        }

        public List<BaseCard> Adventures
        {
            get { return Cards.Where(c => c.Type == Type.Adventure).ToList(); }
        }

        public List<BaseCard> Characters
        {
            get { return Cards.Where(c => c.Type == Type.Character).ToList(); }
        }

        public List<BaseCard> NonHealingCards
        {
            get { return Cards.Where(c => c.HasTag(Tag.Healing) == false).ToList(); }
        }

        public List<BaseCard> CardsExceptStartingCharacter
        {
            get { return Cards.Where(c => c != c.Player.Deck.StartingCharacter).ToList(); }
        }
        #endregion

        public abstract void Add(BaseCard card);
        public abstract void AddAll(IEnumerable<BaseCard> cards);

        protected abstract void Remove(BaseCard card);
        protected abstract void RemoveAll(IEnumerable<BaseCard> cards);
        
        protected CardCollection()
        {
            Cards = new List<BaseCard>();
        }

        protected void MoveToThisCollection(BaseCard card)
        {
            if (card.CurrentCollection.Contains(card))
            {
                card.CurrentCollection.Remove(card);
            }
            else if (card.PreviousCollection != null && card.PreviousCollection.Contains(card))
            {
                card.PreviousCollection.Remove(card);
            }

            card.CurrentCollection = this;
        }

        protected void MoveToThisCollection(IEnumerable<BaseCard> cards)
        {
            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            var cardCollection = cardList.First().CurrentCollection;
            var prevCollection = cardList.First().PreviousCollection;

            if (cardList.All(c => c.CurrentCollection.Contains(c) && c.CurrentCollection == cardCollection))
            {
                cardCollection.RemoveAll(cardList);
            }
            else if (cardList.All(c => c.PreviousCollection != null && c.PreviousCollection.Contains(c)))
            {
                prevCollection.RemoveAll(cardList);
            }

            foreach (var card in cardList)
            {
                card.CurrentCollection = this;
            }
        }

        public BaseCard GetRandomCard()
        {
            return Cards.Skip(Random.Range(0, Cards.Count)).First();
        }

        public BaseCard GetRandomCard(Type ofType)
        {
            var cardsOfType = Cards.Where(c => c.Type == ofType).ToList();
            return cardsOfType.Skip(Random.Range(0, cardsOfType.Count)).First();
        }

        private bool Contains(BaseCard card)
        {
            return Cards.Contains(card);
        }
    }
}
