using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class StimuliDataFrame
{
   public double UnixTimeStamp;
   
   public bool FixationPointOnSet;
   public bool FixationPointOfSet;
   
   public bool StimuliOnset;
   public bool StimuliOffset;

   public bool HeadMovementStimuliOnSet;
   public bool HeadMovementStimuliOffSet;
   public string HeadMovementObjectName;

   public bool SpacePressed;

   public bool trailStarted;
   public bool trialEnded;
}
