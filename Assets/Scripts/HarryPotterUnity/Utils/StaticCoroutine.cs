using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public class StaticCoroutine : MonoBehaviour
    {

        private static StaticCoroutine _instance;
        private static StaticCoroutine Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine ??
                             new GameObject("TweenQueueManager").AddComponent<StaticCoroutine>();

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        private IEnumerator Perform(IEnumerator coroutine)
        {
            yield return this.StartCoroutine(coroutine);
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
            if (_instance != null)
            {
                _instance.StopAllCoroutines();
                Destroy(_instance.gameObject);
            }
            _instance = null;
        }

        [UsedImplicitly]
        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}