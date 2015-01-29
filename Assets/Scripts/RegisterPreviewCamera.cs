using UnityEngine;

namespace Assets.Scripts
{
    public class RegisterPreviewCamera : MonoBehaviour {

        public void Start () {
            Helper.PreviewCamera = GetComponent<Camera>();
        }

    }
}
