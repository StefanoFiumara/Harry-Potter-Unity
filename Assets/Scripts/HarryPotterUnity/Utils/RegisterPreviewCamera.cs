using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class RegisterPreviewCamera : MonoBehaviour {

        public void Start () {
            UtilManager.PreviewCamera = GetComponent<Camera>();
        }

    }
}
