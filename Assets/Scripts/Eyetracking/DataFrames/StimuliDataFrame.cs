using System;
using UnityEngine;

[Serializable]
public class StimuliDataFrame
{
   public string TrialName;
   
   public double UnixTimeStamp;
   
   public bool FixationPointActive;
   public Vector3 FixationPointPosition;
   
   public bool GlobalFixationPointActive;
   public Vector3 GlobalFixationPointPosition;
   
   public bool StimuliActive;

   public bool HeadMovementStimuliActive;
   public string HeadMovementObjectName;

   public string ContrastVariationName;

   public bool SpacePressed;

   public bool TrialActive;
}
