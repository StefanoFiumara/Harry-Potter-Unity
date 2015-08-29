using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.Cards.Generic
{
    public class GenericCreature : GenericCard, IPersistentCard {

        [Header("Creature Settings")]
        [SerializeField, UsedImplicitly]
        protected int _damagePerTurn;

        [SerializeField, UsedImplicitly]
        protected int _health;

        private GameObject _uiCanvas;
        private Text _healthLabel;
        protected Text _attackLabel;
        
        [UsedImplicitly]
        protected override void Start()
        {
            base.Start();
            LoadUiOverlay();
        }

        private void LoadUiOverlay()
        {
            var resource = Resources.Load("CreatureUIOverlay");
            _uiCanvas = (GameObject) Instantiate(resource);
            
            _uiCanvas.transform.position = transform.position - Vector3.back;
            _uiCanvas.transform.SetParent(transform, true);
            _uiCanvas.transform.localRotation = Player.IsLocalPlayer ? Quaternion.identity : Quaternion.Euler(0f,0f,180f);

            _healthLabel = _uiCanvas.transform.FindChild("HealthLabel").gameObject.GetComponent<Text>();
            _attackLabel = _uiCanvas.transform.FindChild("AttackLabel").gameObject.GetComponent<Text>();

            _healthLabel.text = _health.ToString();
            _attackLabel.text = _damagePerTurn.ToString();

            _uiCanvas.SetActive(false);
        }

        protected override void OnClickAction(List<GenericCard> targets)
        {
            Player.InPlay.Add(this);
            Player.Hand.Remove(this);
        }

        public void OnEnterInPlayAction()
        {
            Player.CreaturesInPlay++;
            Player.DamagePerTurn += _damagePerTurn;

            State = CardStates.InPlay;

            _uiCanvas.SetActive(true);
        }

        public void OnExitInPlayAction()
        {
            Player.CreaturesInPlay--;
            Player.DamagePerTurn -= _damagePerTurn;

            _uiCanvas.SetActive(false);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            _healthLabel.text = Mathf.Clamp(_health, 0, int.MaxValue).ToString();

            if (_health > 0) return;

            Player.InPlay.Remove(this);
            Player.Discard.Add(this);
        }

        public void Heal(int amount)
        {
            _health += amount;
            _healthLabel.text = Mathf.Clamp(_health, 0, int.MaxValue).ToString();
        }

        public bool CanPerformInPlayAction() { return false; }

        //Generic Creatures don't do anything special on these actions
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}
