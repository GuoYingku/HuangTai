using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPresenter<T> : IUIPresenter where T : IUIView
{
    public T view;

    public virtual void ShowView()
    {
        view.RootObj.SetActive(true);
    }
    public virtual void HideView()
    {
        view.RootObj.SetActive(false);
    }
}