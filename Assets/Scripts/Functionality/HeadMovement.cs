using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeadMovement
{
    public string MovementType;
    public List<int> MovementPosition;
    public float FixationDuration;    // todo do we add fixation time?
}
