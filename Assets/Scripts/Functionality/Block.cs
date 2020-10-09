using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block
{
   public List<int> SequenceOfTrials;
   
   public List<GridElement> LargeGridClose;
   public List<GridElement> LargeGridFar;
   public List<GridElement> SmallGrid;
   public List<GridElement> SmoothPursuit;
   
   public List<HeadMovement> Roll;
   public List<HeadMovement> Yaw;
   public List<HeadMovement> Pitch;
   
   public List<float> Blink;
   
   public List<Luminance> PupilDilation;
   
   public int FreeViewingIndex;
   public int SmoothPursuitIndex;
}
