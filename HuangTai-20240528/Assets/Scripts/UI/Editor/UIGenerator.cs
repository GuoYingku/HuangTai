using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using Utility;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

public partial class UIGenerator : EditorWindow
{

    private static UIGenerator window;

    private string uiName = "";

    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("UI名称");
            uiName = GUILayout.TextField(uiName);
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Create"))
            {
                if (string.IsNullOrEmpty(uiName))
                {
                    Debug.LogError("Name cannot be null.");
                }
                else
                {
                    if (GenerateNewUI())
                    {
                        window.Close();
                    }
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

    private bool GenerateNewUI()
    {
        if (!CheckGenerateUIScript() || !CheckGenerateUIView())
        {
            return false;
        }

        GenerateUIScript();
        GenerateUIView();

        RegenerateUIID();

        AssetDatabase.Refresh();

        return true;
    }
    private bool CheckGenerateUIScript()
    {
        string dir = string.Format(SCRIPT_DIR_TEMPLATE, uiName);
        if (Directory.Exists(dir))
        {
            Debug.LogError(string.Format(EXISTING_WARNING_TEMPLATE, uiName));
            return false;
        }
        return true;
    }
    private void GenerateUIScript()
    {
        string dir = string.Format(SCRIPT_DIR_TEMPLATE, uiName);

        Directory.CreateDirectory(dir);

        using (var fs = File.CreateText(string.Format(IVIEW_PATH_TEMPLATE, uiName)))
        {
            fs.Write(string.Format(IVIEW_TEMPLATE, uiName));
        }
        using (var fs = File.CreateText(string.Format(IPRESENTER_PATH_TEMPLATE, uiName)))
        {
            fs.Write(string.Format(IPRESENTER_TEMPLATE, uiName));
        }
        using (var fs = File.CreateText(string.Format(VIEW_PATH_TEMPLATE, uiName)))
        {
            fs.Write(string.Format(VIEW_TEMPLATE, uiName));
        }
        using (var fs = File.CreateText(string.Format(PRESENTER_PATH_TEMPLATE, uiName)))
        {
            fs.Write(string.Format(PRESENTER_TEMPLATE, uiName));
        }
    }

    private bool CheckGenerateUIView()
    {
        string resDir = string.Format(RES_DIR_TEMPLATE, uiName);
        if (Directory.Exists(resDir))
        {
            Debug.LogError(string.Format(EXISTING_WARNING_TEMPLATE, uiName));
            return false;
        }
        return true;
    }

    private void GenerateUIView()
    {
        string resDir = string.Format(RES_DIR_TEMPLATE, uiName);
        Directory.CreateDirectory(resDir);
        string resPath = string.Format(VIEW_PREFAB_PATH_TEMPLATE, uiName);
        AssetDatabase.CopyAsset(VIEW_PREFAB, resPath);
        AssetDatabase.ImportAsset(resPath);
        AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(ADDRESSABLE_SETTING_PATH);
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
        }
        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(resPath), settings.DefaultGroup);
    }

    private static void RegenerateUIID()
    {
        SortedSet<string> viewNames = new SortedSet<string>();
        if (!Directory.Exists(RES_PARENT_DIR))
        {
            Directory.CreateDirectory(RES_PARENT_DIR);
        }
        foreach (string viewName in Directory.GetDirectories(RES_PARENT_DIR))
        {
            viewNames.Add(viewName.Substring(RES_PARENT_DIR.Length + 1));
        }
        if (!Directory.Exists(SCRIPT_PARENT_DIR))
        {
            Directory.CreateDirectory(SCRIPT_PARENT_DIR);
        }
        foreach (string viewName in Directory.GetDirectories(SCRIPT_PARENT_DIR))
        {
            viewNames.Add(viewName.Substring(SCRIPT_PARENT_DIR.Length + 1));
        }

        if (!Directory.Exists(SCRIPT_PARENT_DIR))
        {
            Directory.CreateDirectory(SCRIPT_PARENT_DIR);
        }
        using (var fs = File.CreateText(UIID_PATH))
        {
            StringBuilder builder = new StringBuilder();
            foreach (var name in viewNames)
            {
                builder.AppendLine(string.Format(UIID_LINE_TEMPLATE, name));
            }
            fs.Write(string.Format(UIID_TEMPLATE, builder.ToString()));
        }
    }

    [MenuItem("Tools/UI/UI生成器")]
    public static void CreateTestSceneMenuItem()
    {
        if (window == null)
        {
            window = GetWindow<UIGenerator>();
            window.minSize = new Vector2(300, 100);
            window.maxSize = new Vector2(600, 100);
        }
        window.Show();
    }

    [MenuItem("Tools/UI/更新UIID")]
    public static void RegenerateUIIDMenu()
    {
        if (!EditorUtility.DisplayDialog("警告", "这会重新构建UIID类，请注意该操作的潜在影响。", "确定", "取消"))
        {
            return;
        }
        RegenerateUIID();
        AssetDatabase.Refresh();
    }
}