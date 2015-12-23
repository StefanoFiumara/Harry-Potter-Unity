using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.Cards
{
    [SelectionBase]
    public abstract class BaseCard : MonoBehaviour {
        
        #region Inspector Layout
        [Header("Deck Generation Options")]
        [SerializeField, UsedImplicitly] private ClassificationTypes _classification;
        [SerializeField, UsedImplicitly] private Rarity _rarity;

        [Header("Basic Card Settings")]
        [UsedImplicitly] public List<Tag> Tags;
        #endregion

        #region Properties
        private CardCollection _collection;
        public CardCollection PreviousCollection { get; private set; }
        public CardCollection CurrentCollection
        {
            get
            {
                if (_collection == null)
                {
                    throw new Exception("collection is null for card: " + CardName);
                }
                return _collection;
            }
            set
            {
                PreviousCollection = _collection;
                _collection = value;
            }

        }

        public State State { private get; set; }
        public ClassificationTypes Classification { get { return _classification; } }

        public Type Type { get { return GetCardType(); } }
        protected abstract Type GetCardType();

        public FlipState FlipState { private get; set; }
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

        private int ActionCost
        {
            get
            {
                switch (GetCardType())
                {
                    case Type.Adventure:
                    case Type.Character:
                        return 2;
                    default:
                        return 1;
                }
            }
        }

        public byte NetworkId { get; set; }
        public string CardName { get { return string.Format("{0}: {1}", Type, transform.name.Replace("(Clone)", "")); } }
        #endregion

        #region Private Variables
        private InputGatherer _inputGatherer;
        private int _inputRequired;
        
        private static readonly Vector2 ColliderSize = new Vector2(50f, 70f);

        private GameObject _cardFace;
        private GameObject _outline;
        private GameObject _highlight;

        private List<ICardPlayRequirement> _playRequirements;
        private List<IDeckGenerationRequirement> _deckGenerationRequirements;
        #endregion

        [UsedImplicitly]
        protected virtual void Start()
        {
            FlipState = FlipState.FaceDown;
            
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace = transform.FindChild("Front").gameObject;

            AddCollider();

            _inputGatherer = GetComponent<InputGatherer>();

            LoadPlayRequirements();

            AddOutlineComponent();
            AddHighlightComponent();
        }

        private void AddOutlineComponent()
        {
            var tmp = Resources.Load("Outline");

            _outline = (GameObject) Instantiate(tmp);
            _outline.transform.position = transform.position + Vector3.back*0.3f;
            _outline.transform.parent = transform;

            _outline.SetActive(false);
        }

        private void AddHighlightComponent()
        {
            var tmp = Resources.Load("Highlight");

            _highlight = (GameObject)Instantiate(tmp);
            _highlight.transform.position = transform.position + Vector3.back * 0.2f;
            _highlight.transform.parent = transform;
            
            _highlight.SetActive(false);
        }

        private void LoadPlayRequirements()
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
            if (_outline.activeSelf == false) return; //Do not call OnMouseUp if cursor has left the object

            if(IsActivatable())
            {
                //TODO: Gather input for InPlay Action
                GameManager.Network.RPC("ExecuteInPlayActionById", PhotonTargets.All, NetworkId);
                    
            }
            else if (IsPlayableFromHand())
            {
                if (_inputRequired > 0)
                {
                    _inputGatherer.GatherInput();
                }
                else
                {
                    GameManager.Network.RPC("ExecutePlayActionById", PhotonTargets.All, NetworkId);
                }
            }   

            _outline.SetActive(false);
        }
        
        private bool IsPlayableFromHand()
        {            
            bool meetsPlayRequirements = _playRequirements.Count == 0 ||
                                     _playRequirements.TrueForAll(req => req.MeetsRequirement());
            
            return Player.IsLocalPlayer &&
                   State == State.InHand &&
                   Player.CanUseActions(ActionCost) &&
                   meetsPlayRequirements &&
                   IsUnique() &&
                   MeetsAdditionalPlayRequirements();
        }

        private bool IsUnique()
        {
            if (!Tags.Contains(Tag.Unique)) return true;

            var allInPlayCards = Player.InPlay.Cards.Concat(
                Player.OppositePlayer.InPlay.Cards)
                .Select(c => c.CardName);

            return allInPlayCards.Contains(CardName) == false;
        }

        public void MouseUpAction(List<BaseCard> targets = null)
        {       
            foreach (var requirement in _playRequirements)
            {
                requirement.OnRequirementMet();
            }

            OnClickAction(targets);

            Player.UseActions(ActionCost);
            
        }

        protected virtual void OnClickAction(List<BaseCard> targets)
        {
            if (this is IPersistentCard)
            {
                Player.InPlay.Add(this);
            }
            else
            {
                throw new Exception("OnClickAction must be overriden in cards that do not implement IPersistentCard!");
            }
        }
        
        private void ShowPreview()
        {
            _cardFace.layer = GameManager.PREVIEW_LAYER;
            
            if (FlipState == FlipState.FaceUp && iTween.Count(gameObject) == 0)
                GameManager.PreviewCamera.ShowPreview(this);
            else HidePreview();
        }
    
        private void HidePreview()
        {
            _cardFace.layer = GameManager.CARD_LAYER;
            GameManager.PreviewCamera.HidePreview();
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

            throw new NotSupportedException("Card with input did not define valid targets");
            
        }

        protected virtual bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public void SetHighlight()
        {
            if (_highlight) _highlight.SetActive(true);
            else Log.Error("Highlight component has not yet been added.");
        }

        public void RemoveHighlight()
        {
            if(_highlight) _highlight.SetActive(false);
        }
    }
}
