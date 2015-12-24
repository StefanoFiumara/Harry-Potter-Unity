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
            get { return _animator.GetBool("IsOpen"); }
            set
            {
                if (value == false)
                {
                    OnHideMenu();
                }
                else
                {
                    OnShowMenu();
                }
                _animator.SetBool("IsOpen", value);
            }
        }

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _canvasGroup = GetComponent<CanvasGroup>();

            var rect = GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = Vector2.zero;
        }

        protected virtual void Update()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open") == false)
            {
                _canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
            }
            else
            {
                _canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
            }
        }

        public virtual void OnShowMenu() { }
        public virtual void OnHideMenu() { }
    }
}

	
