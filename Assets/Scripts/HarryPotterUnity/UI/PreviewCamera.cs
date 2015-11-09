using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    [UsedImplicitly]
    public class PreviewCamera : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private RectTransform _renderTexture;

        private static readonly Vector3 DefaultPreviewCameraPosition = new Vector3(-400, 255, -70);
        
        private static readonly Vector3 HorizontalPreviewPosition = new Vector3(-466f, 274f, 0f);
        private static readonly Vector3 VerticalPreviewPosition = new Vector3(-515f, 221f, 0f);

        private void RotateHorizontal()
        {
            _renderTexture.localPosition = HorizontalPreviewPosition;
            _renderTexture.rotation = Quaternion.Euler(0f,0f,270f);
        }

        private void RotateVertical()
        {
            _renderTexture.localPosition = VerticalPreviewPosition;
            _renderTexture.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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