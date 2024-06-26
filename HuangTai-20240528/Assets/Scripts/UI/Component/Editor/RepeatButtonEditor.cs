using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RepeatButton), true)]
[CanEditMultipleObjects]
public class RepeatButtonEditor : SelectableEditor
{
    SerializedProperty m_AlphaThresholdProperty;
    SerializedProperty m_OnClickProperty;
    SerializedProperty m_OnRepeatProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_AlphaThresholdProperty = serializedObject.FindProperty("m_AlphaThreshold");
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        m_OnRepeatProperty = serializedObject.FindProperty("m_OnRepeat");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_AlphaThresholdProperty);
        EditorGUILayout.PropertyField(m_OnClickProperty);
        EditorGUILayout.PropertyField(m_OnRepeatProperty);
        serializedObject.ApplyModifiedProperties();
    }
}