using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public class StaticCoroutine : MonoBehaviour
    {

        private static StaticCoroutine _mInstance;

        private static StaticCoroutine Instance
        {
            get
            {
                if (_mInstance != null) return _mInstance;
                
                _mInstance = FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine ??
                             new GameObject("TweenQueueManager").AddComponent<StaticCoroutine>();

                return _mInstance;
            }
        }

        [UsedImplicitly]
        void Awake()
        {
            if (_mInstance == null)
            {
                _mInstance = this;
            }
        }

        IEnumerator Perform(IEnumerator coroutine)
        {
            yield return StartCoroutine(coroutine);
            Die();
        }

        /// <summary>
        /// Place your lovely static IEnumerator in here and witness magic!
        /// </summary>
        /// <param name="coroutine">Static IEnumerator</param>
        public static void DoCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(Instance.Perform(coroutine)); //this will launch the coroutine on our instance
        }

        public static void Die()
        {
            if (_mInstance != null)
            {
                _mInstance.StopAllCoroutines();
                Destroy(_mInstance.gameObject);
            }
            _mInstance = null;
        }

        [UsedImplicitly]
        void OnApplicationQuit()
        {
            _mInstance = null;
        }
    }
}