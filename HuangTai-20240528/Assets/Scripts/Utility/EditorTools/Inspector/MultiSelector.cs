using System;
using UnityEngine;

namespace Utility.EditorTools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MultiSelector : PropertyAttribute
    {
        public MultiSelector() { }
    }
}