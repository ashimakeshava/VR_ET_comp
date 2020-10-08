using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    private Block _block;

    
    private void Start()
    {
        _block.LargeGridClose = new List<GridElement>();
        _block.LargeGridFar = new List<GridElement>();
        _block.SmallGrid = new List<GridElement>();
        _block.SmoothPursuit = new List<GridElement>();
        
        _block.Blink = new List<float>();
        
        _block.Pitch = new List<HeadMovement>();
        _block.Yaw = new List<HeadMovement>();
        _block.Roll = new List<HeadMovement>();
        
        _block.PupilDilation = new List<Luminance>();
    }

    void GenerateBlock()
    {

    }
    
    public Block GetBlock()
    {
        return _block;
    }

    public void SetLargeGridClose(List<GridElement> gridElements)
    {
        _block.LargeGridClose = gridElements;
    }
    
    public void SetLargeGridFar(List<GridElement> gridElements)
    {
        _block.LargeGridFar = gridElements;
    }
    
    public void SetSmallGrid(List<GridElement> gridElements)
    {
        _block.SmallGrid = gridElements;
    }
    
    public void SetSmoothPursuit(List<GridElement> gridElements)
    {
        _block.SmoothPursuit = gridElements;
    }
    
    public void SetBlink(List<float> times)
    {
        _block.Blink = times;
    }
    
    public void SetPitch(List<HeadMovement> headMovements)
    {
        _block.Pitch = headMovements;
    }
    
    public void SetYaw(List<HeadMovement> headMovements)
    {
        _block.Yaw = headMovements;
    }
    
    public void SetRoll(List<HeadMovement> headMovements)
    {
        _block.Roll = headMovements;
    }
    
    public void SetPupilDilation(List<Luminance> luminance)
    {
        _block.PupilDilation = luminance;
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
