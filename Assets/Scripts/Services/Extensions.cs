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

    public static string AppendNewLine(this string text) => text + "\n";
    public static string AppendBreak(this string text) => text + "\n\n";
    public static string AppendTab(this string text) => text + "\t";
}