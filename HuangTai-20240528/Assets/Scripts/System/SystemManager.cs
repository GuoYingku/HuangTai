using Utility.DesignPatterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SystemManager : ISingleton<SystemManager>
{
    private List<IStartSystem> _startSystems = new List<IStartSystem>();
    private List<IUpdateSystem> _updateSystems = new List<IUpdateSystem>();
    private List<IExitSystem> _exitSystems = new List<IExitSystem>();
    private Dictionary<Type, ISystem> _systems = new Dictionary<Type, ISystem>();

    public static SystemManager Instance
    {
        get => ISingleton<SystemManager>.Instance;
    }
    public void Init()
    {
        foreach (Type sysType in Utility.Utility.GetAllConcreteSubclasses(typeof(ISystem)))
        {
            ISystem system = null;
            PropertyInfo instanceProperty;
            if ((instanceProperty = sysType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)) != null)
            {
                system = (ISystem)instanceProperty.GetValue(null);
            }
            else
            {
                system = Activator.CreateInstance(sysType) as ISystem;
            }
            system.Init();
            //_systems.Add(sysType, system);
            if (typeof(IStartSystem).IsAssignableFrom(sysType))
            {
                _startSystems.Add(system as IStartSystem);
            }
            if (typeof(IUpdateSystem).IsAssignableFrom(sysType))
            {
                _updateSystems.Add(system as IUpdateSystem);
            }
            if (typeof(IExitSystem).IsAssignableFrom(sysType))
            {
                _exitSystems.Add(system as IExitSystem);
            }
        }
    }
    public T GetSystem<T>() where T : class, ISystem
    {
        if (_systems.TryGetValue(typeof(T), out ISystem output))
        {
            return (T)output;
        }
        return null;
    }
    public void StartFunc()
    {
        foreach (IStartSystem startSys in _startSystems)
        {
            startSys.StartFunc();
        }
    }
    public void UpdateFunc(float deltaTime)
    {
        foreach (IUpdateSystem updateSys in _updateSystems)
        {
            updateSys.UpdateFunc(deltaTime);
        }
    }
    public void ExitFunc()
    {
        foreach (IExitSystem exitSys in _startSystems)
        {
            exitSys.ExitFunc();
        }
    }
}