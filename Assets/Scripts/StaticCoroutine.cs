using UnityEngine;
using System;
using System.Collections;

public class StaticCoroutine : MonoBehaviour
{

    private static StaticCoroutine mInstance = null;

    private static StaticCoroutine instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine;

                if (mInstance == null)
                {
                    mInstance = new GameObject("StaticCoroutine").AddComponent<StaticCoroutine>();
                }
            }
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as StaticCoroutine;
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
        instance.StartCoroutine(instance.Perform(coroutine)); //this will launch the coroutine on our instance
    }

    void Die()
    {
        mInstance = null;
        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        mInstance = null;
    }
}