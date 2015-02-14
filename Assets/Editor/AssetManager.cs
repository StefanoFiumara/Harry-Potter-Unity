using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class AssetManager : MonoBehaviour 
    {
        [MenuItem("AssetDatabase/Add Photon Views To Cards")]
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
    }
}
