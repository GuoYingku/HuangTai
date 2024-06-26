using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModelAnglesDisplayer : MonoBehaviour
{
    public Camera modelCam;
    public Transform stackerArmRotationPoint;
    public Transform materialArmRotationPoint;
    public Transform inclinometerPoint;

    public TMP_Text stackerArmRotationText;
    public TMP_Text materialArmRotationText;
    public TMP_Text inclinometerText;

    public Vector2 stackerArmRotationOffset;
    public Vector2 materialArmRotationOffset;
    public Vector2 inclinometerOffset;

    public void UpdateTexts(float stackerArmRotation, float materialArmRotation, float inclinometer)
    {
        UpdateText(stackerArmRotationPoint, stackerArmRotationText, "堆料回转：", stackerArmRotation, stackerArmRotationOffset);
        UpdateText(materialArmRotationPoint, materialArmRotationText, "取料回转：", materialArmRotation, materialArmRotationOffset);
        UpdateText(inclinometerPoint, inclinometerText, "取料俯仰：", inclinometer, inclinometerOffset);
    }

    void UpdateText(Transform point, TMP_Text text, string prefix, float angle, Vector2 offset)
    {
        Vector3 pointInCam = modelCam.WorldToViewportPoint(point.position);
        pointInCam += new Vector3(offset.x, offset.y);
        Ray ray = modelCam.ViewportPointToRay(pointInCam);
        text.text = prefix + angle.ToString("F2") + "°";
        text.transform.rotation = modelCam.transform.rotation;
        text.transform.position = modelCam.transform.position + ray.direction * 100;
    }
}