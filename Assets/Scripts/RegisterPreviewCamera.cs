using UnityEngine;
using System.Collections;

public class RegisterPreviewCamera : MonoBehaviour {

	public void Start () {
        Helper.PreviewCamera = GetComponent<Camera>();
	}

}
