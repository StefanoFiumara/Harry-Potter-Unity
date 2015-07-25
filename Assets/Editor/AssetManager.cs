using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [UsedImplicitly]
    public class AssetManager : MonoBehaviour 
    {
        [MenuItem("HP-TCG Card Management/Add Outline Prefabs"), UsedImplicitly]
        public static void AddPhotonViews()
        {
            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));

            string outlinePath = AssetDatabase.GetAllAssetPaths().FirstOrDefault(path => path.Contains("Outline.prefab"));

            var outlinePrefab = (GameObject) AssetDatabase.LoadAssetAtPath(outlinePath, typeof (GameObject));

            
            foreach (string path in assetFolderPaths)
            {
                var obj = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                outlinePrefab.transform.parent = obj.transform;

                AssetDatabase.SaveAssets();
            }
        }
    }
}
