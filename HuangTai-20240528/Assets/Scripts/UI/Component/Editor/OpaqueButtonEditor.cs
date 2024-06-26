using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpaqueButton),true)]
[CanEditMultipleObjects]
public class OpaqueButtonEditor : SelectableEditor
{
    SerializedProperty m_OnClickProperty;
    SerializedProperty m_AlphaThresholdProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        m_AlphaThresholdProperty = serializedObject.FindProperty("m_AlphaThreshold");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_AlphaThresholdProperty);
        EditorGUILayout.PropertyField(m_OnClickProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
