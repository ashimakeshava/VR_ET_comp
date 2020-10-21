using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ET_com_EyetrackingDataFrame
{
    public Vector3 fixationPointPosition;
    
    public double UnixTimeStamp;
    public double DeviceTimeStamp;

    public int LeftStatus;
    public int RightStatus;
    public int CombinedStatus;
    
    public float FPS;

    public bool blinkRight;
    public bool blinkLeft;

    public float pupilDilationLeft;
    public float pupilDilationRight;
    
    public Vector3 HeadPosition;   //main Camera Transform
    public Vector3 NoseVector; //HMD foward;
    
    public Vector3 eyePositionCombinedWorld;
    public Vector3 eyePositionCombinedLocal;
    public Vector3 eyeDirectionCombinedWorld;
    public Vector3 eyeDirectionCombinedLocal;
    
    
    public Vector3 eyePositionLeftWorld;
    public Vector3 eyePositionLeftLocal;
    public Vector3 eyeDirectionLeftWorld;
    public Vector3 eyeDirectionLeftLocal;
    
    
    public Vector3 eyePositionRightWorld;
    public Vector3 eyePositionRightLocal;
    public Vector3 eyeDirectionRightWorld;
    public Vector3 eyeDirectionRightLocal;

    public Vector3 HitPositionOnTarget;
    public Vector3 PositionOfTarget;
    
    public Vector3 ValidationErrorLeft;
    
    public Vector3 ValidationErrorRight;
    
    public Vector3 ValidationErrorCombined;
    
    
    //public float DistanceToCenter; //cant be calculated only from the grid position

    public string nameOfObject;

}
