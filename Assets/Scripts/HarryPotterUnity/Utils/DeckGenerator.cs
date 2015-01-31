using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HarryPotterUnity.Cards;
using UnityEngine;

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

        public List<GenericCard> GenerateDeck(List<Lesson.LessonTypes> types)
        {
            if (types.Count != 2 && types.Count != 3)
            {
                throw new Exception(types.Count + " type(s) sent to GenerateDeck, unsupported");
            }

            var deck = new List<GenericCard>();

            switch (types.Count)
            {
                case 2:
                    AddLessonsToDeck(ref deck, types[0], 15);
                    AddLessonsToDeck(ref deck, types[1], 15);
                    break;
                case 3:
                    AddLessonsToDeck(ref deck, types[0], 15);
                    AddLessonsToDeck(ref deck, types[1], 8);
                    AddLessonsToDeck(ref deck, types[2], 7);
                    break;
            }
            //TODO: Finish this function

            return deck;
        }

        private static void AddLessonsToDeck(ref List<GenericCard> deck, Lesson.LessonTypes lessonType, int amount)
        {
            var card = CardLibrary.Where(c => c.Classification == GenericCard.ClassificationTypes.Lesson)
                .First(l => ((Lesson) l).LessonType == lessonType);

            for (var i = 0; i < amount; i++)
            {
                deck.Add(card); //TODO: Test if cloning issues arise.
            }
        }
    }
}