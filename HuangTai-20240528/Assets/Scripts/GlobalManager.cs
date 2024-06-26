using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utility.DesignPatterns;

public class GlobalManager : MonoSingleton<GlobalManager>
{
    private Queue<Action> _crossThreadOperations = new Queue<Action>();

    private void Awake()
    {
        _ = GlobalManager.Instance;
        Application.targetFrameRate = 60;
        SystemManager.Instance.Init();
    }
    private void Start()
    {
        SystemManager.Instance.StartFunc();
    }

    private void Update()
    {
        lock (_crossThreadOperations)
        {
            while (_crossThreadOperations.TryDequeue(out Action action))
            {
                action.Invoke();
            }
        }
        SystemManager.Instance.UpdateFunc(Time.deltaTime);
    }

    protected override void OnDestroy()
    {
        SystemManager.Instance.ExitFunc();
        base.OnDestroy();
    }

    public void AddCrossThreadOperation(Action action)
    {
        lock(_crossThreadOperations)
        {
            _crossThreadOperations.Enqueue(action);
        }
    }
}