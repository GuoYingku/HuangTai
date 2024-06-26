using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.DesignPatterns;

public class StackerReclaimer : MonoBehaviour
{
    [SerializeField]
    private Transform _stackerSection;
    [SerializeField]
    private Transform _reclaimerSection;
    [SerializeField]
    private Transform _scraperSection;
    [SerializeField]
    private Transform _ropeRoot;

    [SerializeField]
    private Transform _triangleRoot;
    [SerializeField]
    private Transform _triangleTop;
    [SerializeField]
    private Transform _triangleMove;

    private Vector2 _triangleRootPos;
    private Vector2 _triangleTopPos;
    private Vector2 _triangleMovePos;
    private float _rootAngle;
    private float _topAngle;
    private float _ropeLength;
    private float _armLength;

    //public float beltRotation;
    //public float scraperRotation;
    //public float scraperPitch;

    private void Awake()
    {
        _triangleRootPos = new Vector2(_triangleRoot.localPosition.z, _triangleRoot.localPosition.y);
        _triangleTopPos = new Vector2(_triangleTop.localPosition.z, _triangleTop.localPosition.y);
        _triangleMovePos = new Vector2(_triangleMove.localPosition.z, _triangleMove.localPosition.y);
        Vector2 ea = _triangleTopPos - _triangleRootPos;
        Vector2 eb = _triangleMovePos - _triangleRootPos;
        Vector2 ec = _triangleMovePos - _triangleTopPos;
        _rootAngle = Mathf.Acos(Vector2.Dot(ea, eb) / (ea.magnitude * eb.magnitude)) * Mathf.Rad2Deg;
        _topAngle = Mathf.Acos(Vector2.Dot(ec, -ea) / (ec.magnitude * ea.magnitude)) * Mathf.Rad2Deg;
        _ropeLength = ec.magnitude;
        _armLength = ea.magnitude;
    }

    private void Update()
    {
        /*
        StackerRotation(DataManager.Instance.GetIntValue(ConstStr.DATA_MATERIAL_STACKING_ROTARY_ANGLE));
        ReclaimerRotation(DataManager.Instance.GetIntValue(ConstStr.DATA_MATERIAL_FETCHING_ROTARY_ANGLE));
        ReclaimerPitch(DataManager.Instance.GetIntValue(ConstStr.DATA_MATERIAL_FETCHING_PITCH_ANGLE));
        */
    }

    private void StackerRotation(float angle)
    {
        _stackerSection.localEulerAngles = new Vector3(0, angle, 0);
    }

    private void ReclaimerPitch(float angle)
    {
        float cntRootAngle = _rootAngle - angle;//arm/sin(move)=rope/sin(root)
        float cntMoveAngle = Mathf.Asin(_armLength * Mathf.Sin(cntRootAngle * Mathf.Deg2Rad) / _ropeLength) * Mathf.Rad2Deg;
        float cntTopAngle = 180 - cntRootAngle - cntMoveAngle;
        float deltaTopAngle = cntTopAngle - _topAngle;
        _ropeRoot.transform.localEulerAngles = new Vector3(deltaTopAngle, 0, 0);
        _scraperSection.localEulerAngles = new Vector3(angle, 0, 0);
    }

    private void ReclaimerRotation(float angle)
    {
        _reclaimerSection.localEulerAngles = new Vector3(0, angle, 0);
    }
}