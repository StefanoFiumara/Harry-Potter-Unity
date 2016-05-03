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

            _player1 = Player;
            _player2 = Player.OppositePlayer;
            LoadUiOverlay();
        }

        private void LoadUiOverlay()
        {
            //Reuse the CreatureUIOverlay for now
            var resource = Resources.Load("CreatureUIOverlay");
            _uiCanvas = (GameObject)Instantiate(resource);

            _uiCanvas.transform.position = transform.position - Vector3.back;
            _uiCanvas.transform.SetParent(transform, true);
            _uiCanvas.transform.localRotation = Player.IsLocalPlayer ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

            //TODO: Chamge Match progress label colors based on local/remote value
            _p1ProgressLabel = _uiCanvas.transform.FindChild("HealthLabel").gameObject.GetComponent<Text>();
            _p2ProgressLabel = _uiCanvas.transform.FindChild("AttackLabel").gameObject.GetComponent<Text>();

            UpdateProgressLabels();

            _uiCanvas.SetActive(false);
        }

        private void UpdateProgressLabels()
        {
            _p1ProgressLabel.text = string.Format("{0}/{1}",_p1Progress, _goal);
            _p2ProgressLabel.text = string.Format("{0}/{1}", _p2Progress, _goal);
        }

        //Ensure no other matches are in play
        protected override bool MeetsAdditionalPlayRequirements()
        {
            return Player.InPlay.Cards
                .Concat(Player.OppositePlayer.InPlay.Cards)
                .Count(c => c.Type == Type.Match) == 0;
        }

        public void OnEnterInPlayAction()
        {
            _uiCanvas.SetActive(true);
            SubscribeToMatchProgressEvents();
        }

        public void OnExitInPlayAction()
        {
            _uiCanvas.SetActive(false);
            UnsubscribeToMatchProgressEvents();
        }

        private void SubscribeToMatchProgressEvents()
        {
            _player1.OnDamageTaken += OnDamageTakenEvent;
            _player2.OnDamageTaken += OnDamageTakenEvent;
        }

        private void UnsubscribeToMatchProgressEvents()
        {
            _player1.OnDamageTaken -= OnDamageTakenEvent;
            _player2.OnDamageTaken -= OnDamageTakenEvent;
        }

        private void OnDamageTakenEvent(BaseCard sourceCard, int amount)
        {
            //BUG: Causes damage to count towards the enemy player when a player plays a card that causes himself to take damage.
            //probably wont fix...not sure if this ever even happens.
            var playerDealingDamage = sourceCard.Player;

            if (playerDealingDamage == _player1)
            {
                IncreasePlayer1Progress(amount);
            }
            else if (playerDealingDamage == _player2)
            {
                IncreasePlayer2Progress(amount);
            }
        }

        private void IncreasePlayer1Progress(int amount)
        {
            _p1Progress += amount;
            UpdateProgressLabels();

            if (_p1Progress >= _goal)
            {
                Player.Discard.Add(this);
                OnPlayerHasWonMatch(winner: _player1, loser: _player2);
            }
        }

        private void IncreasePlayer2Progress(int amount)
        {
            _p2Progress += amount;
            UpdateProgressLabels();

            if (_p2Progress >= _goal)
            {
                Player.Discard.Add(this);
                OnPlayerHasWonMatch(winner: _player2, loser: _player1);
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
