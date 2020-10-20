using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

[Serializable]
public class StimuliDataFrame
{
   public double UnixTimeStamp;
   
   public bool FixationPointActive;
   public Vector3 FixationPointPosition;
   
   public bool StimuliActive;

   public bool HeadMovementStimuliActive;
   public string HeadMovementObjectName;

   public bool SpacePressed;

   public bool TrialActive;
}
