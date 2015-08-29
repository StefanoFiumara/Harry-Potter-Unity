using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    [UsedImplicitly]
    public class RegisterPreviewCamera : MonoBehaviour {

        [UsedImplicitly]
        public void Start () {
            GameManager._previewCamera = GetComponent<Camera>();
        }

    }
}
