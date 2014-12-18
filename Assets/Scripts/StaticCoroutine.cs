﻿using UnityEngine;
using System;
using System.Collections;

public class StaticCoroutine : MonoBehaviour
{

    private static StaticCoroutine _mInstance;

    private static StaticCoroutine Instance
    {
        get
        {
            if (_mInstance == null)
            {
                _mInstance = GameObject.FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine;

                if (_mInstance == null)
                {
                    _mInstance = new GameObject("StaticCoroutine").AddComponent<StaticCoroutine>();
                }
            }
            return _mInstance;
        }
    }

    void Awake()
    {
        if (_mInstance == null)
        {
            _mInstance = this as StaticCoroutine;
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

    void Die()
    {
        _mInstance = null;
        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        _mInstance = null;
    }
}