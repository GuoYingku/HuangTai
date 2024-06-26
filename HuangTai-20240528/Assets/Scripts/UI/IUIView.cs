using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIView
{
    GameObject RootObj { get; }
    void InitUIElements();
}