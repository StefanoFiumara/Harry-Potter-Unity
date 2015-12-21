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

        private static readonly Vector2 HorizontalPreviewPosition = new Vector2(350f, 0f);
        private static readonly Vector2 VerticalPreviewPosition = Vector2.zero;
        
        private void RotateHorizontal()
        {
            _renderTexture.anchoredPosition = HorizontalPreviewPosition;
            _renderTexture.rotation = Quaternion.Euler(0f,0f,270f);
        }

        private void RotateVertical()
        {
            _renderTexture.anchoredPosition = VerticalPreviewPosition;
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