using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BlinkText : MonoBehaviour
{
    public Color normalColor;
    public Color warningColor;
    public float interval = 1f;

    private TMP_Text _text;
    private float _timer = 0;
    private bool _blinking;
    private bool _isWarningColor;

    private TMP_Text Text
    {
        get
        {
            if(_text==null)
            {
                _text = GetComponent<TMP_Text>();
            }
            return _text;
        }
    }
    public bool Blinking
    {
        get => _blinking;
        set
        {
            _blinking = value;
            if (!value)
            {
                _isWarningColor = false;
                Text.color = normalColor;
            }
        }
    }

    private void Update()
    {
        if (_blinking)
        {
            _timer += Time.deltaTime;
            if (_timer > interval)
            {
                _timer = 0;
                _isWarningColor = !_isWarningColor;
                Text.color = (_isWarningColor ? warningColor : normalColor);
                //Debug.Log(_isWarningColor);
            }
        }
    }
}