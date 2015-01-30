using Assets.Scripts.HarryPotterUnity.Game;
using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Cards
{
    public abstract class GenericCard : MonoBehaviour {

        public enum CardStates
        {
            InDeck, InHand, InPlay, Discarded
        }
        public enum CardTypes
        {
            Lesson, Creature, Spell, Item, Location, Match, Adventure, Character
        }
        public enum CostTypes
        {
            CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch
        }

        public CardStates State { get; set; }
        public CardTypes CardType;

        public Player Player;

        private readonly Vector2 _colliderSize = new Vector2(50f, 70f);

        private GameObject _frontPlane;

        public void Start()
        {
            //Add the collider through code instead of through unity so that if it ever changes, we won't need to edit every prefab.
            if(gameObject.collider == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(_colliderSize.x, _colliderSize.y, 0.2f);
            }

            gameObject.layer = UtilManager.CardLayer;

            _frontPlane = transform.FindChild("Front").gameObject;
        }

        public void SwitchState(CardStates newState)
        {
            State = newState;
        }

        public void OnMouseOver()
        {
            ShowPreview();
        }

        public void OnMouseExit()
        {
            HidePreview();
        }

        private void ShowPreview()
        {
            _frontPlane.layer = UtilManager.PreviewLayer;
            if (State != CardStates.InDeck && State != CardStates.Discarded)
            {
                if (ITween.Count(gameObject) == 0)
                {
                    UtilManager.PreviewCamera.transform.rotation = transform.rotation;
                    UtilManager.PreviewCamera.transform.position = transform.position + 2 * Vector3.back;
                }
                else
                {
                    HidePreview();
                }
            }
        }
    
        private void HidePreview()
        {
            _frontPlane.layer = UtilManager.CardLayer;
            UtilManager.PreviewCamera.transform.position = UtilManager.DefaultPreviewCameraPos;
        }

        public void Disable()
        {
            gameObject.layer = UtilManager.IgnoreRaycastLayer;
            _frontPlane.renderer.material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = UtilManager.CardLayer;
            _frontPlane.renderer.material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = UtilManager.CardLayer;
            _frontPlane.renderer.material.color = Color.yellow;
        }
    
    }
}
