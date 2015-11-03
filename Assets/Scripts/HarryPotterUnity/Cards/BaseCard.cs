using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.DeckGenerationRequirements;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace HarryPotterUnity.Cards
{
    [SelectionBase]
    public abstract class BaseCard : MonoBehaviour {
        
        #region Inspector Layout
        [Header("Deck Generation Options")]
        [SerializeField, UsedImplicitly] private ClassificationTypes _classification;
        [SerializeField, UsedImplicitly] private Rarity _rarity;

        [Header("Basic Card Settings")]
        [SerializeField,UsedImplicitly] private Type _type;

        //TODO: Turn into [Flags] and implement bitmasking? YAGNI? Make private and Serialize?
        [UsedImplicitly] public List<Tag> Tags;

        [UsedImplicitly, SerializeField, Range(0, 2)]
        private int _actionCost = 1;
        #endregion
        
        #region Properties
        protected State State { get; set; }
        public ClassificationTypes Classification { get { return _classification; } }
        public Type Type { get { return _type; } }
        public FlipStates FlipState { private get; set; }
        public Rarity Rarity { get { return _rarity; } }
        public Player Player { get; set; }

        public List<IDeckGenerationRequirement> DeckGenerationRequirements
        {
            get
            {
                return _deckGenerationRequirements ??
                       (_deckGenerationRequirements =
                           GetComponents<MonoBehaviour>().OfType<IDeckGenerationRequirement>().ToList());
            }
        }

        public byte NetworkId { get; set; }
        public string CardName { get { return transform.name; } }
        #endregion

        #region Private Variables
        private InputGatherer _inputGatherer;
        private int _inputRequired;
        
        private static readonly Vector2 ColliderSize = new Vector2(50f, 70f);
        private static readonly Vector3 DefaultPreviewCameraPosition = new Vector3(-400, 255, -70);

        private GameObject _cardFace;
        private GameObject _outline;

        private List<ICardPlayRequirement> _playRequirements;
        private List<IDeckGenerationRequirement> _deckGenerationRequirements;
        #endregion

        [UsedImplicitly]
        protected virtual void Start()
        {
            FlipState = FlipStates.FaceDown;
            
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace = transform.FindChild("Front").gameObject;

            AddCollider();

            _inputGatherer = GetComponent<InputGatherer>();

            GetPlayRequirements();

            AddOutlineComponent();
        }

        private void AddOutlineComponent()
        {
            var tmp = Resources.Load("Outline");

            _outline = (GameObject) Instantiate(tmp);
            _outline.transform.position = transform.position + Vector3.back*0.3f;
            _outline.transform.rotation = _cardFace.transform.rotation;
            _outline.transform.parent = transform;

            _outline.SetActive(false);
        }

        private void GetPlayRequirements()
        {
            _playRequirements = GetComponents<MonoBehaviour>().OfType<ICardPlayRequirement>().ToList();

            var inputRequirement = _playRequirements.OfType<InputRequirement>().SingleOrDefault();

            if (inputRequirement == null) return;

            _inputRequired = inputRequirement.InputRequired;
        }

        private void AddCollider()
        {
            if (gameObject.GetComponent<Collider>() != null) return;

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(ColliderSize.x, ColliderSize.y, 0.2f);
        }

        [UsedImplicitly]
        public void SwitchState(State newState)
        {
            State = newState;
        }

        [UsedImplicitly]
        public void OnMouseOver()
        {
            ShowPreview();

            if (IsPlayableFromHand() || IsActivatable())
            {
                _outline.SetActive(true);
            }

        }

        private bool IsActivatable()
        {
            return State == State.InPlay && 
                            ((IPersistentCard)this).CanPerformInPlayAction() && 
                            Player.IsLocalPlayer;
        }

        [UsedImplicitly]
        public void OnMouseExit()
        {
            HidePreview();
            _outline.SetActive(false);
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if(IsActivatable())
            {
                    //TODO: Gather input here if needed for the InPlay Action
                    Player.NetworkManager.photonView.RPC("ExecuteInPlayActionById", PhotonTargets.All, NetworkId);
            }
            else if (IsPlayableFromHand())
            {
                if (_inputRequired > 0)
                {
                    _inputGatherer.GatherInput();
                }
                else
                {
                    Player.NetworkManager.photonView.RPC("ExecutePlayActionById", PhotonTargets.All, NetworkId);
                }
            }   
        }
        
        private bool IsPlayableFromHand()
        {
            bool meetsRequirements = _playRequirements.Count == 0 ||
                                     _playRequirements.TrueForAll(req => req.MeetsRequirement());

            return Player.IsLocalPlayer &&
                   State == State.InHand &&
                   Player.CanUseActions(_actionCost) &&
                   meetsRequirements &&
                   MeetsAdditionalPlayRequirements();
        }

        public void MouseUpAction(List<BaseCard> targets = null)
        {       
            foreach (var requirement in _playRequirements)
            {
                requirement.OnRequirementMet();
            }

            OnClickAction(targets);

            Player.UseActions(_actionCost);
            
        }

        protected abstract void OnClickAction(List<BaseCard> targets);

        private void ShowPreview()
        {
            _cardFace.layer = GameManager.PREVIEW_LAYER;
            
            if (FlipState == FlipStates.FaceDown) return;

            if (iTween.Count(gameObject) == 0)
            {
                GameManager._previewCamera.transform.rotation = transform.rotation;
                GameManager._previewCamera.transform.position = transform.position + 2 * Vector3.back;
            }
            else
            {
                HidePreview();
            }
        }
    
        private void HidePreview()
        {
            _cardFace.layer = GameManager.CARD_LAYER;
            GameManager._previewCamera.transform.position = DefaultPreviewCameraPosition;
        }

        public void Disable()
        {
            gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = Color.white;
        }

        public void SetSelected()
        {
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace.GetComponent<Renderer>().material.color = Color.yellow;
        }

        public virtual List<BaseCard> GetValidTargets()
        {
            if (_inputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new System.NotSupportedException("Card with input did not define valid targets");
            
        }

        protected virtual bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }
    }
}
