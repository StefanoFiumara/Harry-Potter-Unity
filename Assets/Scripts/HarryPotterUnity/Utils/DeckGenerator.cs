using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.HarryPotterUnity.Cards;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class DeckGenerator
    {
        public static List<GenericCard> CardLibrary { get; set; }

        private static void LoadCardLibrary()
        {
            CardLibrary = new List<GenericCard>();

            var prefabPaths =
                AssetDatabase.GetAllAssetPaths()
                    .Where(path => path.EndsWith(".prefab") && path.Contains("Resources/Cards/"));

            foreach (var path in prefabPaths)
            {
                var container = AssetDatabase.LoadAssetAtPath(path, typeof (GameObject)) as GameObject;
                if (container == null)
                {
                    Debug.LogError(string.Format("Failed to load asset at: {0}", path));
                    continue;
                }
                var cardInfo = container.GetComponent<GenericCard>();
                CardLibrary.Add(cardInfo);
            }
        }

        private void AddLessonsToDeck(ref List<GenericCard> deck, Lesson.LessonTypes lessonType, int amount)
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