using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public static class DeckGenerator
    {
        private static List<GenericCard> _cardLibrary;

        private static List<GenericCard> CardLibrary
        {
            get {
                    if (_cardLibrary == null) LoadCardLibrary();
                    return _cardLibrary;
                }
        }

        private static List<GenericCard> _startingCharacters;
         
        private static void LoadCardLibrary()
        {
            _cardLibrary = new List<GenericCard>();
            _startingCharacters = new List<GenericCard>();

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

                if (cardInfo.Type == Type.Character)
                {
                    _startingCharacters.Add(cardInfo);
                }
            }
        }

        public static GenericCard GetRandomStartingCharacter()
        {
            if(_startingCharacters == null) LoadCardLibrary();

            if (_startingCharacters != null)
            {
                return _startingCharacters.Skip(Random.Range(0, _startingCharacters.Count - 1)).First();
            }

            throw new System.Exception("Starting Characters are not loaded!");
        }

        public static IEnumerable<GenericCard> GenerateDeck(List<LessonTypes> types)
        {
            if (types.Count != 2 && types.Count != 3)
            {
                throw new System.Exception(types.Count + " type(s) sent to GenerateDeck, unsupported");
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
            
            throw new System.ArgumentException("Unable to map lesson type");
        }

        private static void AddLessonsToDeck(ref List<GenericCard> deck, LessonTypes lessonType, int amount)
        {
            var card = CardLibrary.Where(c => c.Classification == ClassificationTypes.Lesson)
                .First(l => ((ILessonProvider) l).LessonType == lessonType);

            for (int i = 0; i < amount; i++)
            {
                deck.Add(card);
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

                var deckCopy = deck.ToList();
                
                bool canBeAdded = (card.DeckGenerationRequirements.Count == 0 ||
                                  card.DeckGenerationRequirements.TrueForAll(req => req.MeetsRequirement(deckCopy))) &&
                                  card.MeetsRarityRequirements();
                
                //TODO: Enabled the second check when enough cards have been implemented
                if (canBeAdded == false /* || deck.Count(c => c.Equals(card)) >= 4 */ ) continue;

                deck.Add(card);
                cardsAdded++;
            }
        }

        private static bool MeetsRarityRequirements(this GenericCard card)
        {
            float chanceToAdd = 1f;

            float rng = Random.Range(0f, 1f);

            switch (card.Rarity)
            {
                case Rarity.Common:
                    chanceToAdd = 1f;
                    break;
                case Rarity.Uncommon:
                    chanceToAdd = 0.7f;
                    break;
                case Rarity.Rare:
                    chanceToAdd = 0.5f;
                    break;
                case Rarity.UltraRare:
                    chanceToAdd = 0.3f;
                    break;
            }
            
            return rng <= chanceToAdd;
        }
    }
}