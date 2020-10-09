using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    private Block _block;
    private GridElementsGenerator _gridElementsGenerator;
    
    private void Start()
    {
        _gridElementsGenerator = GetComponent<GridElementsGenerator>();
    }

    public Block GenerateBlock()
    {
        _block = new Block
        {
            LargeGridClose = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetLargeGrid1()),
            LargeGridFar = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetLargeGrid2()),
            SmallGrid = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetSmallGrid()),
            SmoothPursuit = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetSmoothPursuit()),
            
            // TODO
            /*Pitch = new List<HeadMovement>(),
            Yaw = new List<HeadMovement>(),
            Roll = new List<HeadMovement>(),
        
            Blink = new List<float>(),
            PupilDilation = new List<Luminance>()*/
        };

        return _block;
    }
    
    public void SetFreeViewingIndex(int picNum)
    {
        _block.FreeViewingIndex = picNum;
    }
    
    public void SetSmoothPursuitIndex(int seqNum)
    {
        _block.SmoothPursuitIndex = seqNum;
    }
}
