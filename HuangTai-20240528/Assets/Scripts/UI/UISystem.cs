using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Utility;
using Utility.DesignPatterns;

public class UISystem : ISingleton<UISystem>, ISystem
{
    private Dictionary<UIID, IUIPresenter> _uiPresenter = new Dictionary<UIID, IUIPresenter>();
    private Dictionary<UIID, bool> _uiActive = new Dictionary<UIID, bool>();
    private List<GameObject> _canvasList = new List<GameObject>();
    private GameObject _canvasParent;
    private GameObject _canvasPrefab;

    public static UISystem Instance
    {
        get => ISingleton<UISystem>.Instance;
    }

    public void Init()//canvas7º”‘ÿ ß∞‹
    {
        _canvasPrefab = ConstStr.CANVAS_PREFAB.LoadAssetAtAddress<GameObject>();
        _canvasParent = new GameObject("CanvasParent");
        GameObject.DontDestroyOnLoad(_canvasParent);
        //GameObject eventSystem = GameObject.Instantiate(ConstStr.EVENTSYSTEM_PREFAB.LoadAssetAtAddress<GameObject>(), _canvasParent.transform);
        for (int i = 0; i < 10; ++i)
        {
            GameObject canvas = GameObject.Instantiate(_canvasPrefab, _canvasParent.transform);
            canvas.GetComponent<Canvas>().sortingOrder = i;
            _canvasList.Add(canvas);
            canvas.name = "Canvas_" + i;
        }
    }

    public void HideCanvasParent()
    {
        _canvasParent.SetActive(false);
    }

    public void ShowCanvasParent()
    {
        _canvasParent.SetActive(true);
    }



    public void SetActive(UIID id, bool active)
    {
        if (!_uiPresenter.TryGetValue(id, out IUIPresenter uiPresenter))
        {
            GameObject prefab = GetUIPrefabPath(id.Name).LoadAssetAtAddress<GameObject>();
            CanvasIndex[] canvasIndices = (CanvasIndex[])id.ViewType.GetCustomAttributes(typeof(CanvasIndex), false);
            int ind = 0;
            if (canvasIndices.Length != 0)
            {
                ind = canvasIndices[0].canvasIndex;
            }

            GameObject rootObj = GameObject.Instantiate(prefab, _canvasList[ind].transform);
            IUIView view = Activator.CreateInstance(id.ViewType) as IUIView;
            IUIPresenter presenter = Activator.CreateInstance(id.PresenterType) as IUIPresenter;

            uiPresenter = presenter;
            _uiPresenter[id] = uiPresenter;

            id.ViewType.GetField("presenter").SetValue(view, presenter);
            id.PresenterType.GetField("view").SetValue(presenter, view);
            id.ViewType.GetField("_rootObj", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, rootObj);

            view.InitUIElements();
        }
        if (active)
        {
            if (!_uiActive.TryGetValue(id, out bool cntActive) || !cntActive)
            {
                uiPresenter.ShowView();
                _uiActive[id] = true;
            }
        }
        else
        {
            if (!_uiActive.TryGetValue(id, out bool cntActive) || cntActive)
            {
                _uiActive[id] = false;
                uiPresenter.HideView();
            }
        }
    }
    private string GetUIPrefabPath(string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Assets/Res/UI/UI/");
        builder.Append(name);
        builder.Append("/");
        builder.Append(name);
        builder.Append("View.prefab");
        return builder.ToString();
    }
}