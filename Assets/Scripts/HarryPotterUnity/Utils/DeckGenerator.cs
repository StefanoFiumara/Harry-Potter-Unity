using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.HarryPotterUnity.Cards;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class DeckGenerator
    {
        public static Dictionary<string, GenericCard> CardLibrary = new Dictionary<string, GenericCard>();

        private static void LoadCardLibrary()
        {
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
                CardLibrary.Add(cardInfo.CardName, cardInfo);
            }
        }
    }
}
