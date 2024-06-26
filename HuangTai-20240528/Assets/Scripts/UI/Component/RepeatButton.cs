using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepeatButton : OpaqueButton
{
    [Serializable]
    public class ButtonRepeatEvent : UnityEvent<float> { }

    [SerializeField]
    private ButtonRepeatEvent m_OnRepeat = new ButtonRepeatEvent();

    public ButtonRepeatEvent onRepeat
    {
        get => m_OnRepeat;
        set
        {
            m_OnRepeat = value;
        }
    }

    private void Update()
    {
        if (IsPressed())
        {
            m_OnRepeat.Invoke(Time.deltaTime);
        }
    }
}