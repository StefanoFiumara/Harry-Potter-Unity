using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.Cards
{
    public class BaseCreature : BaseCard, IPersistentCard {

        [Header("Creature Settings")]
        [SerializeField, UsedImplicitly]
        protected int _damagePerTurn;

        [SerializeField, UsedImplicitly]
        protected int _health;

        private GameObject _uiCanvas;
        private Text _healthLabel;
        protected Text _attackLabel;
        
        public int DamagePerTurn { get { return _damagePerTurn; } }
        public int Health { get { return _health; } }

        private int _maxHealth;
        protected override void Start()
        {
            base.Start();
            LoadUiOverlay();

            _maxHealth = _health;
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
        
        public virtual void OnEnterInPlayAction()
        {
            _uiCanvas.SetActive(true);
        }

        public virtual void OnExitInPlayAction()
        {
            _uiCanvas.SetActive(false);
        }

        //TODO: Creature.TakeDamage needs source of damage?
        public void TakeDamage(int amount)
        {
            _health -= amount;
            _healthLabel.text = Mathf.Clamp(_health, 0, int.MaxValue).ToString();

            if (_health <= 0)
            {
                Player.Discard.Add(this);
            }
        }

        public void Heal(int amount)
        {
            _health = Mathf.Clamp(_health + amount, 0, _maxHealth);

            _healthLabel.text = _health.ToString();
        }

        public virtual bool CanPerformInPlayAction() { return false; }
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnSelectedAction() { }

        protected sealed override Enums.Type GetCardType()
        {
            return Enums.Type.Creature;
        }
    }
}
