using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ET_com_EyetrackingDataFrame_vive
{
    //

    public Vector3 fixationPointPosition;
    
    
    
    //validity left
    
    public bool Left_DATA_GAZE_ORIGIN_VALIDITY; /** The validity of the origin of gaze of the eye data */
    public bool Left_DATA_GAZE_DIRECTION_VALIDITY; /** The validity of the direction of gaze of the eye data */
    public bool Left_DATA_PUPIL_DIAMETER_VALIDITY; /** The validity of the diameter of gaze of the eye data */
    public bool Left_DATA_EYE_OPENNESS_VALIDITY; /** The validity of the openness of the eye data */
    public bool Left_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY;  /** The validity of normalized position of pupil */
    //public ulong bitmask; // should be order as above (LSB top, MSB bottom) 
    public bool LeftallValid; 
    public bool LeftoriginAndDirectionValid; // combined gaze seems to only have origin and direction set to valid 
    
    //validity right
    
    public bool Right_DATA_GAZE_ORIGIN_VALIDITY; /** The validity of the origin of gaze of the eye data */
    public bool Right_DATA_GAZE_DIRECTION_VALIDITY; /** The validity of the direction of gaze of the eye data */
    public bool Right_DATA_PUPIL_DIAMETER_VALIDITY; /** The validity of the diameter of gaze of the eye data */
    public bool Right_DATA_EYE_OPENNESS_VALIDITY; /** The validity of the openness of the eye data */
    public bool Right_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY;  /** The validity of normalized position of pupil */
    //public ulong bitmask; // should be order as above (LSB top, MSB bottom) 
    public bool RightallValid; 
    public bool RightoriginAndDirectionValid; 
    
     //Eyetracking Extension
    public double UnixTimeStamp;
    //public double TobiiTimeStamp;
    
    
    
    // HMD 
    public Vector3 HmdPosition;
    public Vector3 NoseVector;     //HMD foward;
    public Quaternion HmdRotation;

    // EyeTracking 
    
    public float EyeOpennessLeftSranipal;
    public float EyeOpennessRightSranipal;
    
    public float pupilDiameterMillimetersLeft;
    public float pupilDiameterMillimetersRight;

    public Vector3 EyePositionCombinedWorld;
    public Vector3 EyeDirectionCombinedWorld;
    
    public Vector3 EyePositionCombinedLocal;        //needed?
    public Vector3 EyeDirectionCombinedLocal;


    
    public Vector3 EyePositionLeftWorld;
    public Vector3 EyeDirectionLeftWorld;
    
    public Vector3 EyePositionLeftLocal; //needed?
    public Vector3 EyeDirectionLeftLocal;
    
    public Vector3 EyePositionRightWorld;
    public Vector3 EyePositionRightLocal; //needed?
    
    public Vector3 EyeDirectionRightWorld;
    public Vector3 EyeDirectionRightLocal;
    
    

    //Tobii
    public bool EyeBlinkingWorldLeftTobii;
    public bool EyeBliningWorldRightTobii;
    
    public bool EyeBlinkingLocalLeftTobii;
    public bool EyeBlinkingLocalRightTobii;

    public Vector3 EyePositionWorldCombinedTobii;
    public Vector3 EyePositionLocalCombinedTobii;
    
    public Vector3 EyeDirectionWorldCombinedTobii;
    public Vector3 EyeDirectionLocalCombinedTobii;
    
    
    //experiment
    
    public Vector3 HitPositionOnTarget;
    public Vector3 PositionOfTarget;
    
    public Vector3 ValidationErrorLeft;
    
    public Vector3 ValidationErrorRight;
    
    public Vector3 ValidationErrorCombined;


    // GazeRay hit object 
    
    public List<HitObjectInfo> hitObjects;

    public string nameOfObject;



}

