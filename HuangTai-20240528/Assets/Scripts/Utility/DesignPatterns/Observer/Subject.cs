using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.DesignPatterns
{
    public delegate void ObserverDelegate(object arg);
    public class Subject : ISingleton<Subject>
    {
        public static Subject Instance { get => ISingleton<Subject>.Instance; }
        private Dictionary<string, ObserverDelegate> _observers = new Dictionary<string, ObserverDelegate>();
        public void Register(string subject,ObserverDelegate observer)
        {
            if (!_observers.ContainsKey(subject))
                _observers[subject] = observer;
            else
                _observers[subject] += observer;
        }
        public void Unregister(string subject, ObserverDelegate observer)
        {
            if (_observers.ContainsKey(subject))
                _observers[subject] -= observer;
        }
        public virtual void Notify(string subject, object arg)
        {
            if(_observers.TryGetValue(subject,out ObserverDelegate observer))
            {
                observer?.Invoke(arg);
            }
        }
    }
}