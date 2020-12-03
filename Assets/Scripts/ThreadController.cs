using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadController : MonoBehaviour
{
    private static ThreadController _instance;

    public static ThreadController Instance => _instance;

    Queue<Action> mainThreadActions;

    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        _instance = this;

        mainThreadActions = new Queue<Action>();
    }

    void Update()
    {
        if (mainThreadActions.Count > 0)
        {
            Action action = mainThreadActions.Dequeue();
            action();
        }
    }

    public void StartThreadedFunction(Action action)
    {
        Thread t = new Thread(new ThreadStart(action));
        t.Start();
    }

    public void QueueMainThreadFunction(Action action)
    {
        mainThreadActions.Enqueue(action);
    }
}
