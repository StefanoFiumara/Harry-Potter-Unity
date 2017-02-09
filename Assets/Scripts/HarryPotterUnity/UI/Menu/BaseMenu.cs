using UnityEngine;

namespace HarryPotterUnity.UI.Menu
{
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class BaseMenu : MonoBehaviour
    {
        private Animator _animator;
        private CanvasGroup _canvasGroup;

        public bool IsOpen
        {
            get { return this._animator.GetBool("IsOpen"); }
            set
            {
                if (value == false)
                {
                    this.OnHideMenu();
                }
                else
                {
                    this.OnShowMenu();
                }
                this._animator.SetBool("IsOpen", value);
            }
        }

        protected virtual void Awake()
        {
            this._animator = this.GetComponent<Animator>();
            this._canvasGroup = this.GetComponent<CanvasGroup>();

            var rect = this.GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = Vector2.zero;
        }

        protected virtual void Update()
        {
            if (this._animator.GetCurrentAnimatorStateInfo(0).IsName("Open") == false)
            {
                this._canvasGroup.blocksRaycasts = this._canvasGroup.interactable = false;
            }
            else
            {
                this._canvasGroup.blocksRaycasts = this._canvasGroup.interactable = true;
            }
        }

        public virtual void OnShowMenu() { }
        public virtual void OnHideMenu() { }
    }
}

	
