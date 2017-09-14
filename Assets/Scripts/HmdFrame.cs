using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HmdFrame
{
    public Vector3 eulerRotation;
    public double dspTime;

    public HmdFrame(Vector3 currentRotation, double currentDspTime)
    {
        this.eulerRotation = currentRotation;
        this.dspTime = currentDspTime;
    }
}
