using System;
using UnityEngine;

namespace Utility.EditorTools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneName : PropertyAttribute
    {
        public SceneName() { }
    }
}