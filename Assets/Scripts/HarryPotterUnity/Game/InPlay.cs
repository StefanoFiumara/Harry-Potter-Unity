using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityLogWrapper;
using UnityEngine;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.Game
{
    public class InPlay : CardCollection
    {
        private static readonly Vector3 _lessonPositionOffset = new Vector3(-255f, -60f, 15f);
        private static readonly Vector3 _topRowPositionOffset = new Vector3(-255f, 0f, 15f);
        private static readonly Vector3 _creaturePositionOffset = new Vector3(5f, -60f, 15f);
        private static readonly Vector3 _characterPositionOffset = new Vector3(-356f, -207, 15f);

        private static readonly Vector2 _lessonSpacing = new Vector2(80f, 15f);
        private static readonly Vector2 _topRowSpacing = new Vector2(80f, 0f);
        private static readonly Vector2 _creatureSpacing = new Vector2(80f, 55f);
        private static readonly Vector2 _characterSpacing = new Vector2(80f, 27f);

        public delegate void CardEnteredPlayEvent(BaseCard card);
        public delegate void CardExitedPlayEvent(BaseCard card);

        public event CardEnteredPlayEvent OnCardEnteredPlay;
        public event CardExitedPlayEvent  OnCardExitedPlay;

        private void OnDestroy()
        {
            this.OnCardEnteredPlay = null;
            this.OnCardExitedPlay = null;
        }

        public override void Add(BaseCard card)
        {
            this.Cards.Add(card);
            
            card.transform.parent = this.transform;

            this.TweenCardToPosition(card);

            this.MoveToThisCollection(card);

            ((IPersistentCard) card).OnEnterInPlayAction();

            if (this.OnCardEnteredPlay != null) this.OnCardEnteredPlay(card);
        }

        protected override void Remove(BaseCard card)
        {
            this.Cards.Remove(card);

            this.RearrangeCardsOfType(card.Type);

            ((IPersistentCard) card).OnExitInPlayAction();

            if (this.OnCardExitedPlay != null) this.OnCardExitedPlay(card);
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                this.Add(card);
            }
        }
        
        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            var cardList = cards as IList<BaseCard> ?? cards.ToList();

            foreach (var card in cardList)
            {
                this.Cards.Remove(card);

                ((IPersistentCard)card).OnExitInPlayAction();

                if (this.OnCardExitedPlay != null) this.OnCardExitedPlay(card);
            }

            foreach (var type in cardList.GroupBy(c => c.Type))
            {
                this.RearrangeCardsOfType(type.Key);
            }
        }

        public override BaseCard GetRandomCard()
        {
            return this.CardsExceptStartingCharacter.Skip(Random.Range(0, this.CardsExceptStartingCharacter.Count)).First();
        }

        private void TweenCardToPosition(BaseCard card)
        {
            var tween = new MoveTween
            {
                Target = card.gameObject,
                Position = this.GetTargetPositionForCard(card),
                Time = 0.3f,
                Flip = FlipState.FaceUp,
                Rotate = TweenRotationType.Rotate90,
                OnCompleteCallback = () => card.State = State.InPlay
            };
            GameManager.TweenQueue.AddTweenToQueue( tween );
        }

        private void RearrangeCardsOfType(Type type)
        {
            var targets = 
                type.IsTopRow() ? this.Cards.FindAll(c => c.Type.IsTopRow()) : this.Cards.FindAll(c => c.Type == type);

            var tween = new AsyncMoveTween
            {
                Targets = targets,
                GetPosition = this.GetTargetPositionForCard
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!this.Cards.Contains(card)) return card.transform.localPosition;

            int position = this.Cards.FindAll(c => c.Type == card.Type).IndexOf(card);
            
            var cardPosition = new Vector3();

            switch (card.Type)
            {
                case Type.Lesson:
                    cardPosition = _lessonPositionOffset;
                    cardPosition.x += (position % 3) * _lessonSpacing.x;
                    cardPosition.y -= (int)(position / 3) * _lessonSpacing.y;
                    cardPosition.z -= (int)(position / 3);
                    break;
                case Type.Creature:
                    cardPosition = _creaturePositionOffset;
                    cardPosition.x += (position % 4) * _creatureSpacing.x;
                    cardPosition.y -= (int)(position / 4) * _creatureSpacing.y;
                    cardPosition.z -= (int)(position / 4);
                    break;
                case Type.Character:
                    cardPosition = _characterPositionOffset;
                    cardPosition.x += (position % 2) * _characterSpacing.x * 0.5f;
                    cardPosition.y -= (int) (position/2)*_characterSpacing.y;

                    int zPosOffset = 2*(position/2);
                    if (position%2 == 1)
                    {
                        zPosOffset -= 2;
                    }

                    cardPosition.z -= zPosOffset;
                    break;
                case Type.Item:
                case Type.Location:
                case Type.Adventure:
                case Type.Match:
                    int topRowIndex = this.Cards.FindAll(c => c.Type.IsTopRow()).IndexOf(card);
                    float shrinkFactor = 0.5f;
                    cardPosition = _topRowPositionOffset;
                    cardPosition.x += topRowIndex * _topRowSpacing.x * shrinkFactor;
                    cardPosition.z -= topRowIndex;
                    break;
                default:
                    Log.Error("GetTargetPositionForCard could not identify cardType");
                    break;
            }

            

            return cardPosition;
        }
    }
}
