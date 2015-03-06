using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
using Lessontypes = HarryPotterUnity.Cards.Lesson.LessonTypes;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public class DeckGenerator
    {
        private static List<GenericCard> _cardLibrary;

        private static List<GenericCard> CardLibrary
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

        public static IEnumerable<GenericCard> GenerateDeck(List<Lessontypes> types)
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
                    AddLessonsToDeck(ref deck, types[1], 14);

                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 15);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 15);
                    break;
                case 3:
                    AddLessonsToDeck(ref deck, types[0], 15);
                    AddLessonsToDeck(ref deck, types[1], 8);
                    AddLessonsToDeck(ref deck, types[2], 7);

                    AddCardsToDeck(ref deck, MapLessonType(types[0]), 10);
                    AddCardsToDeck(ref deck, MapLessonType(types[1]), 10);                   
                    AddCardsToDeck(ref deck, MapLessonType(types[2]), 10);
                    break;
            }

            return deck;
        }

        private static GenericCard.ClassificationTypes MapLessonType(Lesson.LessonTypes type)
        {
            switch (type)
            {
                    case Lesson.LessonTypes.Creatures: return GenericCard.ClassificationTypes.CareOfMagicalCreatures;
                    case Lesson.LessonTypes.Charms: return GenericCard.ClassificationTypes.Charms;
                    case Lesson.LessonTypes.Transfiguration: return  GenericCard.ClassificationTypes.Transfiguration;
                    case Lesson.LessonTypes.Quidditch: return  GenericCard.ClassificationTypes.Quidditch;
                    case Lesson.LessonTypes.Potions: return GenericCard.ClassificationTypes.Potions;
            }
            
            throw new ArgumentException("Unable to map lesson type");
        }

        private static void AddLessonsToDeck(ref List<GenericCard> deck, Lesson.LessonTypes lessonType, int amount)
        {
            var card = CardLibrary.Where(c => c.Classification == GenericCard.ClassificationTypes.Lesson)
                .First(l => ((Lesson) l).LessonType == lessonType);

            for (var i = 0; i < amount; i++)
            {
                deck.Add(card);
            }
        }

        private static void AddCardsToDeck(ref List<GenericCard> deck, GenericCard.ClassificationTypes classification, int amount)
        {
            var potentialCards = CardLibrary.Where(c => c.Classification == classification).ToList();

            var cardsAdded = 0;

            while (cardsAdded < amount)
            {
                var selected = Random.Range(0, potentialCards.Count);
                var card = potentialCards[selected];

                //TODO: Enable this check when enough cards are implemented
                //if (deck.Count(c => c.Equals(card)) >= 4) continue;
                
                deck.Add(card);
                cardsAdded++;
            }
        }
    }
}