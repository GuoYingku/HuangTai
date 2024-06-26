using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class CoalInfo : MonoBehaviour
{
    private TMP_Text _typeTxt;
    private TMP_Text _weightTxt;
    private TMP_Text _timeTxt;
    private TMP_Text _warningTxt;

    private Button _closeBtn;

    private void Awake()
    {
        _typeTxt = transform.FindComponent<TMP_Text>("DetailPnl/TypeField/ValueTxt");
        _weightTxt = transform.FindComponent<TMP_Text>("DetailPnl/WeightField/ValueTxt");
        _timeTxt = transform.FindComponent<TMP_Text>("DetailPnl/TimeField/ValueTxt");
        _warningTxt = transform.FindComponent<TMP_Text>("DetailPnl/WarningField/ValueTxt");

        _closeBtn = transform.FindComponent<Button>("CloseButton");
    }
}