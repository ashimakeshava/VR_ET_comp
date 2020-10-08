using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[Serializable]
public class GridElement
{
    public string ObjectName;
    public Vector3 Position;
    public float FixationDuration;
    public float MovementDuration;
}
