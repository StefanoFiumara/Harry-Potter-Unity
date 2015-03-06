using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public abstract class GenericCard : MonoBehaviour {

        public enum CardStates
        {
            InDeck, InHand, InPlay, Discarded
        }
        public enum CardTypes
        {
            Lesson, Creature, Spell // Item, Location, Match, Adventure, Character
        }

        public enum ClassificationTypes
        {
            CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch,
            Lesson
           // Character,
           // Adventure
        }

        [UsedImplicitly, SerializeField]
        protected int ActionCost = 1;

        public CardStates State { get; protected set; }

        [UsedImplicitly] 
        public CardTypes CardType;

        [UsedImplicitly] 
        public ClassificationTypes Classification;
        
        //Might need this later
        /*
        [SerializeField]
        [UsedImplicitly]
        private string _cardName = "";
        */

        public Player Player { get; set; }

        private readonly Vector2 _colliderSize = new Vector2(50f, 70f);

        private GameObject _frontPlane;


        [UsedImplicitly]
        public void Start()
        {
            if(gameObject.GetComponent<Collider>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(_colliderSize.x, _colliderSize.y, 0.2f);
            }

            gameObject.layer = UtilManager.CardLayer;

            _frontPlane = transform.FindChild("Front").gameObject;
        }

        [UsedImplicitly]
        public void SwitchState(CardStates newState)
        {
            State = newState;
        }

        [UsedImplicitly]
        public void OnMouseOver()
        {
            ShowPreview();
        }

        [UsedImplicitly]
        public void OnMouseExit()
        {
            HidePreview();
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if (!Player.IsLocalPlayer) return;
            if (State != CardStates.InHand) return;
            if (!Player.CanUseActions(ActionCost)) return;
            if (!MeetsAdditionalPlayRequirements()) return;

            OnClickAction();

            if (CardType != CardTypes.Spell)
            {
                Player.UseActions(ActionCost);   
            }
        }

        protected abstract void OnClickAction();
        protected abstract bool MeetsAdditionalPlayRequirements();

        private void ShowPreview()
        {
            _frontPlane.layer = UtilManager.PreviewLayer;
            if (State == CardStates.InDeck || State == CardStates.Discarded) return;

            if (iTween.Count(gameObject) == 0)
            {
                UtilManager.PreviewCamera.transform.rotation = transform.rotation;
                UtilManager.PreviewCamera.transform.position = transform.position + 2 * Vector3.back;
            }
            else
            {
                HidePreview();
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
            _frontPlane.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = UtilManager.CardLayer;
            _frontPlane.GetComponent<Renderer>().material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = UtilManager.CardLayer;
            _frontPlane.GetComponent<Renderer>().material.color = Color.yellow;
        }
    
    }
}
