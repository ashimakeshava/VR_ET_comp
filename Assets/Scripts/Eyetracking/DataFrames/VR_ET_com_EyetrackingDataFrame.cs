using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ET_com_EyetrackingDataFrame
{
    public double UnixTimeStamp;
    
    public float FPS;
    
    public Vector3 HeadPosition;   //main Camera Transform
    public Vector3 NoseVector; //HMD foward;
    
    public Vector3 eyePositionWorldCombined;
    public Vector3 eyePositionLocalCombined;
    public Vector3 eyeDirectionWorldCombined;
    public Vector3 eyeDirectionLocalCombined;
    
    
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
    public float Distance;

}
