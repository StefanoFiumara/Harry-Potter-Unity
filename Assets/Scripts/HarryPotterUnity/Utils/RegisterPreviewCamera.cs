using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public class RegisterPreviewCamera : MonoBehaviour {

        public void Start () {
            UtilManager.PreviewCamera = GetComponent<Camera>();
        }

    }
}
