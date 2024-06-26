using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlinkImage : MonoBehaviour
{
    public Sprite normalImage;
    public Sprite warningImage;
    public float interval = 1f;

    private Image _image;
    private float _timer;

    private bool _blinking;
    private bool _isWarningSprite;

    private Image image
    {
        get
        {
            if(_image==null)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }
    public bool Blinking
    {
        get => _blinking;
        set
        {
            _blinking = value;
            if(!value)
            {
                _isWarningSprite = false;
                image.sprite = normalImage;
            }
        }
    }

    void Update()
    {
        if(_blinking)
        {
            _timer += Time.deltaTime;
            if (_timer>interval)
            {
                _timer = 0;
                _isWarningSprite = !_isWarningSprite;
                image.sprite = (_isWarningSprite ? warningImage : normalImage);
            }
        }
    }
}
