using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldWithSlider : MonoBehaviour
{
    public TMP_InputField input;
    public Slider slider;

    void Start()
    {
        input.onEndEdit.AddListener(InputFinishEdit);
        slider.onValueChanged.AddListener(SliderValueChange);
    }

    void InputFinishEdit(string text)
    {
        float val = float.Parse(text);
        val = Mathf.Clamp(val, slider.minValue, slider.maxValue);
        slider.value = val;
    }
    void SliderValueChange(float val)
    {
        input.text = val.ToString();
    }
}