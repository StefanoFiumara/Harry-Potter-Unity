using JetBrains.Annotations;

namespace HarryPotterExtensions
{
    public static class CardProcessing {

        /*[MenuItem("Harry Potter TCG/Do Work")] */
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
