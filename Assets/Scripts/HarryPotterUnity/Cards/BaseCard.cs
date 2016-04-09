using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.Cards
{
    [SelectionBase]
    public abstract class BaseCard : MonoBehaviour
    {
        [Header("Deck Generation Options")]
        [SerializeField, UsedImplicitly] private ClassificationTypes _classification;
        [SerializeField, UsedImplicitly] private Rarity _rarity;

        [Header("Card Settings")]
        [SerializeField, EnumFlags]
        [UsedImplicitly] private Tag _tags; 
        
        public State State { private get; set; }
        public ClassificationTypes Classification { get { return _classification; } set { _classification = value; } }

        public Type Type { get { return GetCardType(); } }
        protected abstract Type GetCardType();

        public FlipState FlipState { private get; set; }
        public Rarity Rarity { get { return _rarity; }  set { _rarity = value; } }

        public Player Player { get; set; }

        private List<IDeckGenerationRequirement> _deckGenerationRequirements; 
        public List<IDeckGenerationRequirement> DeckGenerationRequirements
        {
            get
            {
                return _deckGenerationRequirements ??
                       (_deckGenerationRequirements =
                           GetComponents<MonoBehaviour>().OfType<IDeckGenerationRequirement>().ToList());
            }
        }

        public string CardName { get { return string.Format("{0}: {1}", Type, transform.name.Replace("(Clone)", "")); } }
        public byte NetworkId { get; set; }

        private InputGatherer _inputGatherer;
        private int _fromHandActionInputRequired;
        private int _inPlayActionInputRequired;

        private static readonly Vector2 _colliderSize = new Vector2(50f, 70f);

        private GameObject _cardFace;
        private GameObject _outline;
        private GameObject _highlight;

        private List<ICardPlayRequirement> PlayRequirements { get; set; }

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

        protected virtual void Start()
        {
            FlipState = FlipState.FaceDown;
            
            gameObject.layer = GameManager.CARD_LAYER;
            _cardFace = transform.FindChild("Front").gameObject;

            AddCollider();
            
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
            PlayRequirements = GetComponents<MonoBehaviour>().OfType<ICardPlayRequirement>().ToList();

            var inputRequirement = PlayRequirements.OfType<InputRequirement>().SingleOrDefault();

            if (inputRequirement != null)
            {
                _inputGatherer = GetComponent<InputGatherer>();
                _fromHandActionInputRequired = inputRequirement.FromHandActionInputRequired;
                _inPlayActionInputRequired = inputRequirement.InPlayActionInputRequired;
            }
        }

        private void AddCollider()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(_colliderSize.x, _colliderSize.y, 0.2f);
            }
        }
        
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
                   ((IPersistentCard) this).CanPerformInPlayAction();
        }

        public void OnMouseExit()
        {
            HidePreview();
            _outline.SetActive(false);
        }

        public void OnMouseUp()
        {
            if (_outline.activeSelf == false) return; //Do not call OnMouseUp if cursor has left the object

            if(IsActivatable())
            {
                if (_inPlayActionInputRequired > 0)
                {
                    _inputGatherer.GatherInput(InputGatherMode.InPlayAction);
                }
                else
                {
                    GameManager.Network.RPC("ExecuteInPlayActionById", PhotonTargets.All, NetworkId);
                }
            }
            else if (IsPlayableFromHand())
            {
                if (_fromHandActionInputRequired > 0)
                {
                    _inputGatherer.GatherInput(InputGatherMode.FromHandAction);
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
            bool meetsPlayRequirements = PlayRequirements.Count == 0 ||
                                     PlayRequirements.TrueForAll(req => req.MeetsRequirement());
            
            return Player.IsLocalPlayer &&
                   State == State.InHand &&
                   Player.CanUseActions(ActionCost) &&
                   meetsPlayRequirements &&
                   IsUnique() &&
                   MeetsAdditionalPlayRequirements();
        }

        public bool HasTag(Tag t)
        {
            return (_tags & t) == t;    
        }

        private bool IsUnique()
        {
            if (HasTag(Tag.Unique) == false) return true;

            var allInPlayCards = Player.InPlay.Cards.Concat(
                Player.OppositePlayer.InPlay.Cards)
                .Select(c => c.CardName);

            return allInPlayCards.Contains(CardName) == false;
        }

        public void PlayFromHand(List<BaseCard> targets = null)
        {       
            foreach (var requirement in PlayRequirements)
            {
                requirement.OnRequirementMet();
            }

            OnPlayFromHandAction(targets);

            Player.UseActions(ActionCost);
            
        }

        protected virtual void OnPlayFromHandAction(List<BaseCard> targets)
        {
            if (this is IPersistentCard)
            {
                Player.InPlay.Add(this);
            }
            else
            {
                throw new Exception("OnPlayFromHandAction must be overriden in cards that do not implement IPersistentCard!");
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

        public virtual List<BaseCard> GetFromHandActionTargets()
        {
            if (_fromHandActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new NotSupportedException("Card with from hand input did not define valid targets");
        }

        public virtual List<BaseCard> GetInPlayActionTargets()
        {
            if (_inPlayActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }
            else
            {
                throw new NotSupportedException("Card with in play input did not define valid targets.");
            }
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
