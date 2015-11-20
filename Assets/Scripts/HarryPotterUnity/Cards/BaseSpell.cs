using System.Collections.Generic;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseSpell : BaseCard {

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        protected sealed override void OnClickAction(List<BaseCard> targets)
        {
            Enable();
            PreviewSpell();
            
            Player.Discard.Add(this);

            SpellAction(targets);
        }

        protected abstract void SpellAction(List<BaseCard> targets);

        private void PreviewSpell()
        {
            State = State.Discarded;
            var rotateType = Player.OppositePlayer.IsLocalPlayer ? TweenRotationType.Rotate180 : TweenRotationType.NoRotate;
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(gameObject, SpellOffset, 0.5f, 0f, FlipState.FaceUp, rotateType, State, 0.6f));
        }

        protected override Type GetCardType()
        {
            return Type.Spell;
        }
    }
}