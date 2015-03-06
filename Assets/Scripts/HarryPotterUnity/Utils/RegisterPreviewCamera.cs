using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public class RegisterPreviewCamera : MonoBehaviour {

        [UsedImplicitly]
        public void Start () {
            UtilManager.PreviewCamera = GetComponent<Camera>();
        }

    }
}
