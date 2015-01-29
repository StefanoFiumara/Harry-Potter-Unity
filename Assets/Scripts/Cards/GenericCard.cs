using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Cards
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

        public CardStates State;
        public CardTypes CardType;

        public Player _Player;

        private readonly Vector2 COLLIDER_SIZE = new Vector2(50f, 70f);

        private GameObject FrontPlane;

        public void Start()
        {
            //Add the collider through code instead of through unity so that if it ever changes, we won't need to edit every prefab.
            if(gameObject.collider == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(COLLIDER_SIZE.x, COLLIDER_SIZE.y, 0.2f);
            }

            gameObject.layer = Helper.CardLayer;

            FrontPlane = transform.FindChild("Front").gameObject;
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
            FrontPlane.layer = Helper.PreviewLayer;
            if (State != CardStates.InDeck && State != CardStates.Discarded)
            {
                if (iTween.Count(gameObject) == 0)
                {
                    Helper.PreviewCamera.transform.rotation = transform.rotation;
                    Helper.PreviewCamera.transform.position = transform.position + 2 * Vector3.back;
                }
                else
                {
                    HidePreview();
                }
            }
        }
    
        private void HidePreview()
        {
            FrontPlane.layer = Helper.CardLayer;
            Helper.PreviewCamera.transform.position = Helper.DefaultPreviewCameraPos;
        }

        public void Disable()
        {
            gameObject.layer = Helper.IgnoreRaycastLayer;
            FrontPlane.renderer.material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = Helper.CardLayer;
            FrontPlane.renderer.material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = Helper.CardLayer;
            FrontPlane.renderer.material.color = Color.yellow;
        }
    
    }
}
