using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class CanvasIndex : Attribute
{
    public int canvasIndex;
    public CanvasIndex(int ind)
    {
        if (ind < 0)
        {
            ind = 0;
        }
        if (ind > 9)
        {
            ind = 9;
        }
        canvasIndex = ind;
    }
}