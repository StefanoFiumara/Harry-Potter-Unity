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
        
        public State State { get; set; }
        public ClassificationTypes Classification { get { return this._classification; } set { this._classification = value; } }

        public Type Type { get { return this.GetCardType(); } }
        protected abstract Type GetCardType();

        public FlipState FlipState { private get; set; }
        public Rarity Rarity { get { return this._rarity; }  set { this._rarity = value; } }

        public Player Player { get; set; }

        private List<IDeckGenerationRequirement> _deckGenerationRequirements; 
        public List<IDeckGenerationRequirement> DeckGenerationRequirements
        {
            get
            {
                return this._deckGenerationRequirements ??
                       (this._deckGenerationRequirements = this.GetComponents<MonoBehaviour>().OfType<IDeckGenerationRequirement>().ToList());
            }
        }

        public string CardName { get { return string.Format("{0}: {1}", this.Type, this.transform.name.Replace("(Clone)", "")); } }
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
                if (this._collection == null)
                {
                    throw new Exception("collection is null for card: " + this.CardName);
                }
                return this._collection;
            }
            set
            {
                this.PreviousCollection = this._collection;
                this._collection = value;
            }

        }

        private int ActionCost
        {
            get
            {
                switch (this.GetCardType())
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
            this.FlipState = FlipState.FaceDown;

            this.gameObject.layer = GameManager.CARD_LAYER;
            this._cardFace = this.transform.FindChild("Front").gameObject;

            this.AddCollider();

            this.LoadPlayRequirements();

            this.AddOutlineComponent();
            this.AddHighlightComponent();
        }

        private void AddOutlineComponent()
        {
            var tmp = Resources.Load("Outline");

            this._outline = (GameObject) Instantiate(tmp);
            this._outline.transform.position = this.transform.position + Vector3.back*0.3f;
            this._outline.transform.parent = this.transform;

            this._outline.SetActive(false);
        }

        private void AddHighlightComponent()
        {
            var tmp = Resources.Load("Highlight");

            this._highlight = (GameObject)Instantiate(tmp);
            this._highlight.transform.position = this.transform.position + Vector3.back * 0.2f;
            this._highlight.transform.parent = this.transform;

            this._highlight.SetActive(false);
        }

        private void LoadPlayRequirements()
        {
            this.PlayRequirements = this.GetComponents<MonoBehaviour>().OfType<ICardPlayRequirement>().ToList();

            var inputRequirement = this.PlayRequirements.OfType<InputRequirement>().SingleOrDefault();

            if (inputRequirement != null)
            {
                this._inputGatherer = this.GetComponent<InputGatherer>();
                this._fromHandActionInputRequired = inputRequirement.FromHandActionInputRequired;
                this._inPlayActionInputRequired = inputRequirement.InPlayActionInputRequired;
            }
        }

        private void AddCollider()
        {
            if (this.gameObject.GetComponent<Collider>() == null)
            {
                var col = this.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(_colliderSize.x, _colliderSize.y, 0.2f);
            }
        }
        
        public void OnMouseOver()
        {
            this.ShowPreview();

            if ( (this.IsPlayableFromHand() || this.IsActivatable()) && GameManager.IsInputGathererActive == false)
            {
                this._outline.SetActive(true);
            }

        }

        public void OnMouseExit()
        {
            this.HidePreview();
            this._outline.SetActive(false);
        }

        public void OnMouseUp()
        {
            if (this._outline.activeSelf == false) return; //Do not call OnMouseDown if cursor has left the object

            if(GameManager.IsInputGathererActive) return; //Player clicked on this card as a target, not to activate its effect.

            if(this.IsActivatable())
            {
                if (this._inPlayActionInputRequired > 0)
                {
                    this._inputGatherer.GatherInput(InputGatherMode.InPlayAction);
                }
                else
                {
                    GameManager.Network.RPC("ExecuteInPlayActionById", PhotonTargets.All, this.NetworkId);
                }
            }
            else if (this.IsPlayableFromHand())
            {
                if (this._fromHandActionInputRequired > 0)
                {
                    this._inputGatherer.GatherInput(InputGatherMode.FromHandAction);
                }
                else
                {
                    GameManager.Network.RPC("ExecutePlayActionById", PhotonTargets.All, this.NetworkId);
                }
            }

            this._outline.SetActive(false);
        }

        private bool IsActivatable()
        {
            return this.State == State.InPlay 
                   && ((IPersistentCard) this).CanPerformInPlayAction()
                   && this.GetInPlayActionTargets().Count >= this._fromHandActionInputRequired;
        }

        private bool IsPlayableFromHand()
        {            
            bool meetsPlayRequirements = this.PlayRequirements.Count == 0 || this.PlayRequirements.All(req => req.MeetsRequirement());

            bool meetsPlayerConstraints = this.Player.Constraints.Count == 0 || this.Player.Constraints.All(req => req.MeetsConstraint(this));

            return this.Player.IsLocalPlayer && this.State == State.InHand && this.Player.CanUseActions(this.ActionCost) &&
                   meetsPlayRequirements &&
                   meetsPlayerConstraints && this.IsUnique() && this.MeetsAdditionalPlayRequirements();
        }

        public bool HasTag(Tag t)
        {
            return (this._tags & t) == t;    
        }

        private bool IsUnique()
        {
            if (this.HasTag(Tag.Unique) == false) return true;

            var allInPlayCards = this.Player.InPlay.Cards.Concat(this.Player.OppositePlayer.InPlay.Cards)
                .Select(c => c.CardName);

            return allInPlayCards.Contains(this.CardName) == false;
        }

        public void PlayFromHand(List<BaseCard> targets = null)
        {       
            foreach (var requirement in this.PlayRequirements)
            {
                requirement.OnRequirementMet();
            }

            this.OnPlayFromHandAction(targets);

            this.Player.UseActions(this.ActionCost);
            
        }

        protected virtual void OnPlayFromHandAction(List<BaseCard> targets)
        {
            if (this is IPersistentCard)
            {
                this.Player.InPlay.Add(this);
            }
            else
            {
                throw new Exception("OnPlayFromHandAction must be overriden in cards that do not implement IPersistentCard!");
            }
        }
        
        private void ShowPreview()
        {
            this._cardFace.layer = GameManager.PREVIEW_LAYER;
            
            if (this.FlipState == FlipState.FaceUp && iTween.Count(this.gameObject) == 0)
                GameManager.PreviewCamera.ShowPreview(this);
            else this.HidePreview();
        }
    
        private void HidePreview()
        {
            this._cardFace.layer = GameManager.CARD_LAYER;
            GameManager.PreviewCamera.HidePreview();
        }

        public void Disable()
        {
            this.gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            this._cardFace.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        }

        public void Enable()
        {
            this.gameObject.layer = GameManager.CARD_LAYER;
            this._cardFace.GetComponent<Renderer>().material.color = Color.white;
        }

        public void SetSelected()
        {
            this.gameObject.layer = GameManager.CARD_LAYER;
            this._cardFace.GetComponent<Renderer>().material.color = Color.yellow;
        }

        public virtual List<BaseCard> GetFromHandActionTargets()
        {
            if (this._fromHandActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new NotSupportedException("Card with from hand input did not define valid targets");
        }

        public virtual List<BaseCard> GetInPlayActionTargets()
        {
            if (this._inPlayActionInputRequired == 0)
            {
                return new List<BaseCard>();
            }

            throw new NotSupportedException("Card with in play input did not define valid targets.");
        }

        protected virtual bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public void SetHighlight()
        {
            if (this._highlight) this._highlight.SetActive(true);
            else Log.Error("Highlight component has not yet been added.");
        }

        public void RemoveHighlight()
        {
            if(this._highlight) this._highlight.SetActive(false);
        }
    }
}
