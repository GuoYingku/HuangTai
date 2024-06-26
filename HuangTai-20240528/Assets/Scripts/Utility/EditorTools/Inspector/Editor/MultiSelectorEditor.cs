using UnityEngine;
using UnityEditor;
using System;

namespace Utility.EditorTools
{
    [CustomPropertyDrawer(typeof(MultiSelector))]
    public class MultiSelectorEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Enum targetObject = Enum.ToObject(fieldInfo.FieldType, property.enumValueFlag) as Enum;
            targetObject = EditorGUI.EnumFlagsField(position, label, targetObject);
            property.enumValueFlag = Convert.ToInt32(targetObject);
            if(GUI.changed)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
}