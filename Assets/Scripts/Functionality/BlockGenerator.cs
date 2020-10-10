using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = System.Random;

public class BlockGenerator : MonoBehaviour
{
    private Block _block;
    private Random _random;
    private GridElementsGenerator _gridElementsGenerator;
    [ReadOnly] private readonly List<int> _trialsToShuffle = new List<int> {2,3,4,5,6,7,8,9,10};
    
    private void Start()
    {
        _gridElementsGenerator = GetComponent<GridElementsGenerator>();
        _random = new Random();
    }

    public Block GenerateBlock(GameObject picture, List<GridElement> smoothPursuit)
    {
        _block = new Block
        {
            SequenceOfTrials = RandomizeTrials(),
            
            LargeGridClose = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetLargeGrid1()),
            LargeGridFar = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetLargeGrid2()),
            SmallGrid = _gridElementsGenerator.Traverse(ExperimentManager.Instance.GetSmallGrid()),
            
            // todo take the smooth pursuit list from experiment manager (from and already randomized list for that)
            SmoothPursuit = smoothPursuit,
            
            
            
            // todo take the free viewing from experiment manager (from and already randomized list for that)
            // todo fill out the rest
        };
        
        
        
        return _block;
    }

    private List<int> RandomizeTrials()
    {
        List<int> list = new List<int> {1};

        for (int i = 0; i < _trialsToShuffle.Count+1; i++)
        {
            int index = _random.Next(_trialsToShuffle.Count);
            list.Add(_trialsToShuffle[index]);
            _trialsToShuffle.RemoveAt(index);
        }
        
        list.Add(1);

        return list;
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
