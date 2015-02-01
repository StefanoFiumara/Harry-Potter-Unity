using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HarryPotterUnity.Cards;
using UnityEngine;

using ClassificationTypes = Assets.Scripts.HarryPotterUnity.Cards.GenericCard.ClassificationTypes;
using LessonTypes = Assets.Scripts.HarryPotterUnity.Cards.Lesson.LessonTypes;
using Random = UnityEngine.Random;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class DeckGenerator
    {
        private static List<GenericCard> _cardLibrary;

        public static List<GenericCard> CardLibrary
        {
            get {
                    if (_cardLibrary == null) LoadCardLibrary();
                    return _cardLibrary;
                }
        }

        private static void LoadCardLibrary()
        {
            _cardLibrary = new List<GenericCard>();

            var resources = Resources.LoadAll("Cards/");
            
            foreach (var container in resources.Cast<GameObject>())
            {
                if (container == null)
                {
                    Debug.LogError("Failed to load asset");
                    continue;
                }
                var cardInfo = container.GetComponent<GenericCard>();
                _cardLibrary.Add(cardInfo);
            }
        }

        public static List<GenericCard> GenerateDeck(List<LessonTypes> types)
        {
            if (types.Count != 2 && types.Count != 3)
            {
                throw new Exception(types.Count + " type(s) sent to GenerateDeck, unsupported");
            }

            var deck = new List<GenericCard>();

            switch (types.Count)
            {
                case 2:
                    AddLessonsToDeck(ref deck, types[0], 16);
                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 15);
                    AddLessonsToDeck(ref deck, types[1], 14);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 15);
                    break;
                case 3:
                    AddLessonsToDeck(ref deck, types[0], 15);
                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 10);
                    AddLessonsToDeck(ref deck, types[1], 8);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 10);
                    AddLessonsToDeck(ref deck, types[2], 7);
                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 10);
                    break;
            }
            //TODO: Finish this function

            return deck;
        }

        private static ClassificationTypes MapLessonType(LessonTypes type)
        {
            switch (type)
            {
                    case LessonTypes.Creatures: return ClassificationTypes.CareOfMagicalCreatures;
                    case LessonTypes.Charms: return ClassificationTypes.Charms;
                    case LessonTypes.Transfiguration: return  ClassificationTypes.Transfiguration;
                    case LessonTypes.Quidditch: return  ClassificationTypes.Quidditch;
                    case LessonTypes.Potions: return ClassificationTypes.Potions;
            }
            
            throw new ArgumentException("Unable to map lesson type");
        }

        private static void AddLessonsToDeck(ref List<GenericCard> deck, LessonTypes lessonType, int amount)
        {
            var card = CardLibrary.Where(c => c.Classification == ClassificationTypes.Lesson)
                .First(l => ((Lesson) l).LessonType == lessonType);

            for (var i = 0; i < amount; i++)
            {
                deck.Add(card); //TODO: Test if cloning issues arise.
            }
        }

        private static void AddCardsToDeck(ref List<GenericCard> deck, ClassificationTypes classification, int amount)
        {
            var potentialCards = CardLibrary.Where(c => c.Classification == classification).ToList();

            int cardsAdded = 0;

            while (cardsAdded < amount)
            {
                int selected = Random.Range(0, potentialCards.Count);
                var card = potentialCards[selected];

                //TODO: Enable this check when enough cards are implemented
                //if (deck.Count(c => c.Equals(card)) >= 4) continue;
                
                deck.Add(card);
                cardsAdded++;
            }
        }
    }
}