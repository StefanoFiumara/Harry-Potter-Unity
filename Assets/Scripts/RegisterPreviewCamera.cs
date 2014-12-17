using UnityEngine;

public class RegisterPreviewCamera : MonoBehaviour {

	public void Start () {
        Helper.PreviewCamera = GetComponent<Camera>();
	}

}
