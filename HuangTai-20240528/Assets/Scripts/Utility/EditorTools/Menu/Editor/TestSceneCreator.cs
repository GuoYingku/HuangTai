using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace Utility.EditorTools
{
    public class TestSceneCreator : EditorWindow
    {
        public const string FILE_PATH = "CT_EP_TEST_SCENE_DIR";
        public const string defaultFilePath = "Scenes/TestScene/";
        static TestSceneCreator window;
        [MenuItem("Tools/CreateTestScene")]
        public static void CreateTestSceneMenuItem()
        {
            if (window == null)
            {
                window = GetWindow<TestSceneCreator>();
                window.minSize = new Vector2(300, 100);
                window.maxSize = new Vector2(600, 100);
            }
            window.Show();
        }
        static string fileName = "";
        private void CreateGUI()
        {
            if (!EditorPrefs.HasKey(FILE_PATH))
                EditorPrefs.SetString(FILE_PATH, defaultFilePath);
        }
        private void OnGUI()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(EditorPrefs.GetString(FILE_PATH), GUILayout.ExpandWidth(false));
                fileName = GUILayout.TextField(fileName).Replace("/", "");
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create"))
                {
                    if (fileName == "")
                    {
                        Debug.LogError("Scene name cannot be null.");
                    }
                    else
                    {
                        CreateTestScene();
                        window.Close();
                    }
                }
                if (GUILayout.Button("Close"))
                {
                    window.Close();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
        private void OnDestroy()
        {
            fileName = "";
        }
        void CreateTestScene()
        {
            string dir = "/" + EditorPrefs.GetString(FILE_PATH) + "/" + fileName;
            string temp = (Application.dataPath + dir).Replace("//", "/");
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
            EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive), ("Assets/" + dir + "/" + fileName).Replace("//", "/") + ".unity");
        }
    }
}