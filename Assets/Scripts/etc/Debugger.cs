using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debugger : MonoBehaviour {

    public static void OutputList<T>(string title, List<T> toOut)
    {
        string toPrint = title +":  ";
        foreach (T t in toOut)
            toPrint += t.ToString() + ", ";
        Debug.Log(toPrint);
    }

    public static void OutputList<T>(string title, T[] toOut)
    {
        string toPrint = title + ":  ";
        for (int i = 0; i < toOut.Length; ++i)
        {
            toPrint += toOut[i] + ", ";
        }
        Debug.Log(toPrint);
    }
}
