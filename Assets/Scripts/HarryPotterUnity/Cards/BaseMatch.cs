using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseMatch : BaseCard, IPersistentCard
    {
        private Player _player1;
        private Player _player2;

        private int _p1Progress;
        private int _p2Progress;
        
        private GameObject _uiCanvas;
        private Text _p1ProgressLabel;
        private Text _p2ProgressLabel;

        [Header("Match Settings")]
        [SerializeField, UsedImplicitly]
        private int _goal;

        public abstract void OnPlayerHasWonMatch(Player winner, Player loser);

        protected override void Start()
        {
            base.Start();

            this._player1 = this.Player;
            this._player2 = this.Player.OppositePlayer;
            this.LoadUiOverlay();
        }

        private void LoadUiOverlay()
        {
            //Reuse the CreatureUIOverlay for now
            var resource = Resources.Load("CreatureUIOverlay");
            this._uiCanvas = (GameObject)Instantiate(resource);

            this._uiCanvas.transform.position = this.transform.position - Vector3.back;
            this._uiCanvas.transform.SetParent(this.transform, true);
            this._uiCanvas.transform.localRotation = this.Player.IsLocalPlayer ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

            //TODO: Change Match progress label colors based on local/remote value
            this._p1ProgressLabel = this._uiCanvas.transform.FindChild("HealthLabel").gameObject.GetComponent<Text>();
            this._p2ProgressLabel = this._uiCanvas.transform.FindChild("AttackLabel").gameObject.GetComponent<Text>();

            this.UpdateProgressLabels();

            this._uiCanvas.SetActive(false);
        }

        private void UpdateProgressLabels()
        {
            this._p1ProgressLabel.text = string.Format("{0}/{1}", this._p1Progress, this._goal);
            this._p2ProgressLabel.text = string.Format("{0}/{1}", this._p2Progress, this._goal);
        }

        //Ensure no other matches are in play
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return this.Player.InPlay.Cards
                .Concat(this.Player.OppositePlayer.InPlay.Cards)
                .Count(c => c.Type == Type.Match) == 0;
        }

        public void OnEnterInPlayAction()
        {
            this._uiCanvas.SetActive(true);
            this.SubscribeToMatchProgressEvents();
        }

        public void OnExitInPlayAction()
        {
            this._uiCanvas.SetActive(false);
            this.UnsubscribeToMatchProgressEvents();
        }

        private void SubscribeToMatchProgressEvents()
        {
            this._player1.OnDamageTakenEvent += this.OnDamageTakenEvent;
            this._player2.OnDamageTakenEvent += this.OnDamageTakenEvent;
        }

        private void UnsubscribeToMatchProgressEvents()
        {
            this._player1.OnDamageTakenEvent -= this.OnDamageTakenEvent;
            this._player2.OnDamageTakenEvent -= this.OnDamageTakenEvent;
        }

        private void OnDamageTakenEvent(BaseCard sourceCard, int amount)
        {
            //BUG: Causes damage to count towards the enemy player when a player plays a card that causes himself to take damage.
            var cardOwner = sourceCard.Player;
            
            if (cardOwner == this._player1)
            {
                this.IncreasePlayer1Progress(amount);
            }
            else if (cardOwner == this._player2)
            {
                this.IncreasePlayer2Progress(amount);
            }
        }

        private void IncreasePlayer1Progress(int amount)
        {
            this._p1Progress += amount;
            this.UpdateProgressLabels();

            if (this._p1Progress >= this._goal)
            {
                this.Player.Discard.Add(this);
                this.OnPlayerHasWonMatch(winner: this._player1, loser: this._player2);
            }
        }

        private void IncreasePlayer2Progress(int amount)
        {
            this._p2Progress += amount;
            this.UpdateProgressLabels();

            if (this._p2Progress >= this._goal)
            {
                this.Player.Discard.Add(this);
                this.OnPlayerHasWonMatch(winner: this._player2, loser: this._player1);
            }
        }
        
        protected sealed override Type GetCardType()
        {
            return Type.Match;
        }

        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnInPlayAction(List<BaseCard> targets = null) { }
        public virtual bool CanPerformInPlayAction() { return false; }
    }
}
