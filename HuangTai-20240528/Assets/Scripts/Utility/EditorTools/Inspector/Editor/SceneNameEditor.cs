using UnityEngine;
using UnityEditor;
using System;

namespace Utility.EditorTools
{
    [CustomPropertyDrawer(typeof(SceneName))]
    public class SceneNameEditor : PropertyDrawer
    {
        static GUIContent[] scenes;
        GUIContent[] GetSceneNames()
        {
            GUIContent[] g = new GUIContent[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < g.Length; ++i)
            {
                string[] splitResult = EditorBuildSettings.scenes[i].path.Split('/');
                string nameWithSuffix = splitResult[splitResult.Length - 1];
                g[i] = new GUIContent(nameWithSuffix.Substring(0, nameWithSuffix.Length - ".unity".Length));
            }
            return g;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            scenes = GetSceneNames();
            string cntString = property.stringValue;
            int selected = 0;
            string targetScene;
            for (int i = 1; i < scenes.Length; ++i)
            {
                if (scenes[i].text.Equals(cntString))
                {
                    selected = i;
                    break;
                }
            }
            selected = EditorGUI.Popup(position, label, selected, scenes);
            targetScene = scenes[selected].text;
            property.stringValue = targetScene;
            if (GUI.changed)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
}