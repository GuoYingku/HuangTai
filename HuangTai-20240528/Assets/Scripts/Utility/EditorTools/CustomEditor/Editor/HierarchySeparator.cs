using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utility.EditorTools
{
    [InitializeOnLoad]
    public static class HierarchySeparator
    {
        public const string prefix = "<<";
        static GUIStyle fontStyle;
        static HierarchySeparator()
        {
            Enable();
        }
        static void Draw(int instanceID, Rect selectionRect)
        {
            GameObject g = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!g)
                return;
            if (g.name.StartsWith(prefix))
            {
                Rect renderRect = selectionRect;
                EditorGUI.DrawRect(renderRect, new Color(0.6f, 0.85f, 0.42f));
                EditorGUI.LabelField(renderRect, g.name.Substring(2), fontStyle);
            }
        }
        static void Enable()
        {
            EditorApplication.hierarchyWindowItemOnGUI += Draw;
            fontStyle = new GUIStyle
            {
                normal = { textColor = Color.black },
                alignment = TextAnchor.MiddleCenter,
            };
        }
        static void Disable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= Draw;
        }
    }
}