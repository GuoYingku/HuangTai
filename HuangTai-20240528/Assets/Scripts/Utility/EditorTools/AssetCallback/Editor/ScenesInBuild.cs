using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utility.EditorTools
{
    public class ScenesInBuild : AssetModificationProcessor
    {
        static void OnWillCreateAsset(string assetName)
        {
            string testScenePath = "Assets/";
            if (EditorPrefs.HasKey(TestSceneCreator.FILE_PATH))
                testScenePath += EditorPrefs.GetString(TestSceneCreator.FILE_PATH);
            else
                testScenePath += TestSceneCreator.defaultFilePath;
            if (assetName.EndsWith(".unity.meta") && !assetName.StartsWith(testScenePath))
            {
                assetName = assetName.Substring(0, assetName.Length - 5);
                EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(assetName, true);
                var scenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
                var prevScenes = EditorBuildSettings.scenes;
                for (int i = 0; i < prevScenes.Length; ++i)
                {
                    if (prevScenes[i].path == assetName)
                        return;
                    scenes[i] = prevScenes[i];
                }
                scenes[scenes.Length - 1] = newScene;
                EditorBuildSettings.scenes = scenes;
            }
        }
        static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
        {
            if (assetName.EndsWith(".unity"))
            {
                var prevScenes = EditorBuildSettings.scenes;
                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
                for (int i = 0; i < prevScenes.Length; ++i)
                {
                    if (prevScenes[i].path != assetName)
                        scenes.Add(prevScenes[i]);
                }
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}