using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [UsedImplicitly]
    public class AssetManager : MonoBehaviour 
    {
        /*[MenuItem("HP-TCG Card Management/Do Work")] */
        /// <summary>
        /// Template for applying changes to all card objects in the library
        /// </summary>
        [UsedImplicitly]
        public static void DoWork()
        {
            /*
            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));
            
            foreach (string path in assetFolderPaths)
            {
                var obj = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                //Do work with asset...

                AssetDatabase.SaveAssets();
            }
            */
        }
    }
}
