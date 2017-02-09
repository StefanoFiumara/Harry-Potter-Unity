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
        
        public int DamagePerTurn { get { return this._damagePerTurn; } }

        public int MaxHealth { get; private set; }

        protected override void Start()
        {
            base.Start();
            this.LoadUiOverlay();

            this.MaxHealth = this._health;
        }

        private void LoadUiOverlay()
        {
            var resource = Resources.Load("CreatureUIOverlay");
            this._uiCanvas = (GameObject) Instantiate(resource);

            this._uiCanvas.transform.position = this.transform.position - Vector3.back;
            this._uiCanvas.transform.SetParent(this.transform, true);
            this._uiCanvas.transform.localRotation = this.Player.IsLocalPlayer ? Quaternion.identity : Quaternion.Euler(0f,0f,180f);

            this._healthLabel = this._uiCanvas.transform.FindChild("HealthLabel").gameObject.GetComponent<Text>();
            this._attackLabel = this._uiCanvas.transform.FindChild("AttackLabel").gameObject.GetComponent<Text>();

            this._healthLabel.text = this._health.ToString();
            this._attackLabel.text = this._damagePerTurn.ToString();

            this._uiCanvas.SetActive(false);
        }
        
        public virtual void OnEnterInPlayAction()
        {
            this._uiCanvas.SetActive(true);
        }

        public virtual void OnExitInPlayAction()
        {
            this._uiCanvas.SetActive(false);
        }

        public void TakeDamage(int amount)
        {
            this._health -= amount;
            this._healthLabel.text = Mathf.Clamp(this._health, 0, int.MaxValue).ToString();

            if (this._health <= 0)
            {
                this.Player.Discard.Add(this);
            }
        }

        public void Heal(int amount)
        {
            this._health = Mathf.Clamp(this._health + amount, 0, this.MaxHealth);

            this._healthLabel.text = this._health.ToString();
        }

        public virtual bool CanPerformInPlayAction() { return false; }
        public virtual void OnInPlayBeforeTurnAction() { }
        public virtual void OnInPlayAfterTurnAction() { }
        public virtual void OnInPlayAction(List<BaseCard> targets = null) { }

        protected sealed override Type GetCardType()
        {
            return Type.Creature;
        }
    }
}
