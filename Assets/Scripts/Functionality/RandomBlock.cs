using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomBlock
{
   public List<GridElement> LargeGridClose;
   public List<GridElement> LargeGridFar;
   public List<GridElement> SmallGrid;
   public List<GridElement> SmoothPursuit;
   public List<float> Blink;
   public List<Luminance> PupilDialation;
   public List<HeadMovement> Roll;
   public List<HeadMovement> Yaw;
   public List<HeadMovement> Pitch;
   public int FreeViewingindex;
   public int SmoothPursuitIndex;
}
