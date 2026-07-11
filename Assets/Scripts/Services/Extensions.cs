using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
    public static Coroutine ExecuteDelayed(this MonoBehaviour monoBehaviour, Action action)
    {
        return monoBehaviour.StartCoroutine(Routine());
        IEnumerator Routine()
        {
            yield return null;
            action?.Invoke();
        }
    }
}