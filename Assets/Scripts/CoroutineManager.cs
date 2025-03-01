using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public class CoroutineObject
    {
        public IEnumerator routine;
        public bool isCompleted;

        public Action SetCoroutineCompleted;

        public CoroutineObject(IEnumerator Routine)
        {
            routine = Routine;
            isCompleted = false;
            SetCoroutineCompleted = () => isCompleted = true;
        }
    }

    private List<CoroutineObject> _activeCoroutines = new List<CoroutineObject>();

    public int GetActiveCoroutinesCount()
    {
        return _activeCoroutines.Count;
    }

    public void StartCoroutineProcess(IEnumerator coroutine)
    {
        CoroutineObject routine = new CoroutineObject(coroutine);
        _activeCoroutines.Add(routine);
        StartCoroutine(ProceedCoroutine(routine));
    }
    public IEnumerator ProceedCoroutine(CoroutineObject coroutine)
    {
        yield return StartCoroutine(coroutine.routine);
        coroutine.SetCoroutineCompleted();
    }

    bool IsAllCoroutinesCompleted()
    {   
        return _activeCoroutines.All(routine => routine.isCompleted);
    }

    public IEnumerator CheckAllCoroutinesCompleted(Action coroutineCallbackAction)
    {
        yield return new WaitUntil(() => IsAllCoroutinesCompleted());
        coroutineCallbackAction();
        _activeCoroutines.Clear();
    }

    public void SkipCoroutineProcess(Action callbackAction)
    {
        callbackAction();
    }
}