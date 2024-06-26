using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OpaqueButton : Button
{
    [SerializeField]
    private float m_AlphaThreshold = 0.1f;
    protected override void Start()
    {
        image.alphaHitTestMinimumThreshold = m_AlphaThreshold;
    }
}