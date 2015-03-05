using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class AssetManager : MonoBehaviour 
    {
        [MenuItem("HP-TCG Card Management/Add Photon Views To Cards")]
        public static void AddPhotonViews()
        {
            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));

            foreach (var path in assetFolderPaths)
            {
                var obj = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                var view = obj.GetComponent<PhotonView>();
                if (view)
                {
                    if (!view.ObservedComponents.Contains(obj.transform))
                    {
                        view.ObservedComponents.Add(obj.transform);
                        Debug.Log("View already created but Observed was not set");
                    }
                }
                else
                {
                    obj.AddComponent<PhotonView>();
                    obj.GetComponent<PhotonView>().ObservedComponents.Add(obj.transform);
                    Debug.Log("Created View and Set Observed");
                }
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("HP-TCG Card Management/Remove Photon Views from Cards")]
        public static void RemovePhotonViews()
        {
            var assetFolderPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") && path.Contains("/Cards/"));

            foreach (var path in assetFolderPaths)
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
