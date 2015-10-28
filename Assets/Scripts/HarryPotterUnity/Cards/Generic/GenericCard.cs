using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.Cards.Generic.PlayRequirements;
using HarryPotterUnity.Game;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    [SelectionBase]
    public abstract class GenericCard : MonoBehaviour {

        #region Enums
        public enum CardStates
        {
            InDeck, InHand, InPlay, Discarded
        }

        public enum CardType
        {
            Lesson, Creature, Spell, Item, Location, Match, Adventure, Character
        }

        public enum Tag
        {
            Unique, Healing, Wand, Cauldron, Broom
        }

        public enum CardRarity
        {
            Common, Uncommon, Rare, UltraRare
        }
        
        public enum ClassificationTypes
        {
            CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch,
            Lesson,
            Character,
            Adventure
        }

        public enum FlipStates
        {
            FaceUp, FaceDown
        }
        #endregion

        #region Inspector Layout
        [Header("Deck Generation Options")]
        [SerializeField, UsedImplicitly] private ClassificationTypes _classification;
        [SerializeField, UsedImplicitly] private CardRarity _rarity;

        [Header("Basic Card Settings")]
        [SerializeField,UsedImplicitly] private CardType _cardType;

        //TODO: Turn into [Flags] and implement bitmasking? YAGNI? Make private and Serialize?
        [UsedImplicitly] public List<Tag> Tags;

        [UsedImplicitly, SerializeField, Range(0, 2)]
        private int _actionCost = 1;
        #endregion
        
        #region Properties
        public CardStates State { get; set; }
        public ClassificationTypes Classification { get { return _classification; } }
        public CardType Type { get { return _cardType; } }
        public FlipStates FlipState { get; set; }
        public CardRarity Rarity { get { return _rarity; } }
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
        public void SwitchState(CardStates newState)
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
            return State == CardStates.InPlay && 
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
            if(IsActivateable())
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
                   State == CardStates.InHand &&
                   Player.CanUseActions(_actionCost) &&
                   meetsRequirements &&
                   MeetsAdditionalPlayRequirements();
        }

        public void MouseUpAction(List<GenericCard> targets = null)
        {       
            foreach (var requirement in _playRequirements)
            {
                requirement.OnRequirementMet();
            }

            OnClickAction(targets);

            Player.UseActions(_actionCost);
            
        }

        protected abstract void OnClickAction(List<GenericCard> targets);

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

        public virtual List<GenericCard> GetValidTargets()
        {
            if (_inputRequired == 0)
            {
                return new List<GenericCard>();
            }

            throw new NotSupportedException("Card with input did not define valid targets");
            
        }

        protected virtual bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }
    }
}
