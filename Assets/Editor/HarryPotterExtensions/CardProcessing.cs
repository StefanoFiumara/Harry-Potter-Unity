using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration.Requirements;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HarryPotterExtensions
{
    public static class CardProcessing
    {    
        /// <summary>
        /// Template for applying changes to all card objects in the library
        /// </summary>
        [UsedImplicitly]
        //[MenuItem("Harry Potter TCG/Find Requirement")]
        public static void DoWork()
        {

            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));

            foreach (string path in assetFolderPaths)
            {
                var obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                if (obj.GetComponent<DeckDoesNotContainCardWithNameRequirement>() != null)
                {
                    Debug.Log(obj.name);
                }
            }
        }
    }
}
