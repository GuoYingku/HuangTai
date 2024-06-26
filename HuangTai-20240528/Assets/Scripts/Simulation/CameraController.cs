using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _minDist = 350;
    [SerializeField]
    private float _maxDist = 600;
    [SerializeField]
    private float _zoomSpeed = 20;

    [SerializeField]
    private float _minPitchAngle = 40;
    [SerializeField]
    private float _maxPitchAngle = 80;
    [SerializeField]
    private float _rotateSpeed = 50;

    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private Transform _cameraWrapper;

    private float _cntDist;
    private Vector3 _cntEuler;

    private bool _prevMouseDown;
    private Vector3 _prevMousePos;

    private void Start()
    {
        _cntDist = -_cameraTransform.localPosition.z;
    }

    void Update()
    {
        Zoom();
        Rotate();
    }

    void Zoom()
    {
        _cntDist -= Input.mouseScrollDelta.y * _zoomSpeed;
        _cntDist = Mathf.Min(Mathf.Max(_cntDist, _minDist), _maxDist);
        _cameraTransform.localPosition = new Vector3(0, 0, -_cntDist);
    }

    void Rotate()
    {
        if (!Input.GetMouseButton(1))
        {
            _prevMouseDown = false;
            return;
        }
        if (!_prevMouseDown)
        {
            _prevMouseDown = true;
            _cntEuler = _cameraWrapper.localEulerAngles;
        }
        else
        {
            Vector3 delta = Camera.main.ScreenToViewportPoint(Input.mousePosition) - _prevMousePos;
            _cntEuler.y += delta.x * _rotateSpeed;
            _cntEuler.x -= delta.y * _rotateSpeed;
            _cntEuler.x = Mathf.Min(Mathf.Max(_cntEuler.x, _minPitchAngle), _maxPitchAngle);
            _cameraWrapper.localEulerAngles = _cntEuler;
        }
        _prevMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    
    }
}