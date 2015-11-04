using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    [UsedImplicitly]
    public class PreviewCamera : MonoBehaviour
    {
        private GameObject _renderTexture;

        private static readonly Vector3 DefaultPreviewCameraPosition = new Vector3(-400, 255, -70);

        [UsedImplicitly]
        private void Start()
        {
            //TODO: Switch to using HUD element for preview camera
            _renderTexture = GameObject.Find("PreviewRenderTexture");

            if(_renderTexture == null) throw new System.Exception("Preview Camera could not find render texture GameObject");
        }

        private void RotateHorizontal()
        {
            _renderTexture.transform.rotation = Quaternion.Euler(0f,0f,90);
        }

        private void RotateVertical()
        {
            _renderTexture.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        public void ShowPreview(BaseCard card)
        {
            if (card is IPersistentCard) RotateHorizontal();
            else RotateVertical();
            
            transform.rotation = card.transform.rotation;
            transform.position = card.transform.position + 2 * Vector3.back;

            _renderTexture.gameObject.SetActive(true);
        }

        public void HidePreview()
        {
            _renderTexture.gameObject.SetActive(false);
            RotateVertical();
            transform.position = DefaultPreviewCameraPosition;
        }
    }
}