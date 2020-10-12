using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;


public class RandomGenerator : MonoBehaviour
{
    private Block _block;
    private Random _random;
    private RouteGenerator _routeGenerator;
    [ReadOnly] private readonly List<int> _trialsToShuffle = new List<int> {2,3,4,5,6,7,8,9,10};
    
    private void Start()
    {
        _routeGenerator = GetComponent<RouteGenerator>();
        _random = new Random();
    }

    public Block GenerateBlock(List<FreeViewingDataFrame> freeViewingDataFrames, List<GridElement> smoothPursuit)
    {
        _block = new Block
        {
            SequenceOfTrials = RandomizeTrials(),
            
            LargeGridClose = _routeGenerator.GetGridRoute(),
            LargeGridFar = _routeGenerator.GetGridRoute(),
            SmallGrid = _routeGenerator.GetGridRoute(),
            
            // todo take the smooth pursuit list from experiment manager (from and already randomized list for that)
            SmoothPursuit = smoothPursuit,
            FreeViewingPictureList = freeViewingDataFrames,
            /*Blink = ;*/
            PupilDilation = _routeGenerator.RandomisePupilDilationDataFrame(),
            PupilDilationBlackFixationDuration = 7f + RandomUnity.Range(-.25f, .25f),
            
            
            
            // todo take the free viewing from experiment manager (from and already randomized list for that)
            // todo fill out the rest
        };
        
        
        
        return _block;
    }

    private List<int> RandomizeTrials()
    {
        List<int> list = new List<int> {0, 1};

        for (int i = 0; i < _trialsToShuffle.Count+1; i++)
        {
            int index = _random.Next(_trialsToShuffle.Count);
            list.Add(_trialsToShuffle[index]);
            _trialsToShuffle.RemoveAt(index);
        }
        
        list.Add(1);

        return list;
    }
}
