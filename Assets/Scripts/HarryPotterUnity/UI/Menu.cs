using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class Menu : MonoBehaviour
    {
        private Animator _animator;
        private CanvasGroup _canvasGroup;

        public bool IsOpen
        {
            get { return _animator.GetBool("IsOpen"); }
            set
            { _animator.SetBool("IsOpen", value); }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _canvasGroup = GetComponent<CanvasGroup>();

            var rect = GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = Vector2.zero;
        }

        private void Update()
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

    }
}

	
