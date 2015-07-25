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
                //var outlineInstance = (GameObject) Instantiate(outlinePrefab, new Vector3(0f, 0f, 0.1f), Quaternion.Euler(90f, -180f, 0));

                outlinePrefab.transform.parent = obj.transform;

                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("HP-TCG Card Management/Remove Photon Views from Cards"), UsedImplicitly]
        public static void RemovePhotonViews()
        {
            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));

            foreach (string path in assetFolderPaths)
            {
                var obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                var view = obj.GetComponent<PhotonView>();
                if (view)
                {
                    DestroyImmediate(obj.GetPhotonView(), true);
                    Debug.Log("Destroyed photon view");
                }
                else
                {
                    Debug.Log("No photon view found on card");
                }
                AssetDatabase.SaveAssets();
            }
        }
    }
}
