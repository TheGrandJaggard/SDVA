using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour // TODO this needs to use my NonSingletonController system
{
    public static CoroutineRunner instance;

    private void Awake()
    {
        instance = this;
    }

    public Coroutine Run(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void Stop(Coroutine routine)
    {
        StopCoroutine(routine);
    }
}
