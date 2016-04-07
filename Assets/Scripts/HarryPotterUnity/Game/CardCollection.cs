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

        private bool Contains(BaseCard card)
        {
            return Cards.Contains(card);
        }
    }
}
