using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;


public class RandomGenerator : MonoBehaviour
{
    [Header("Constant Trials between all Participants")] [Space]
    [SerializeField] private List<GameObject> freeViewingPictures;
    
    private Random _random;
    private RouteGenerator _routeGenerator;
    [ReadOnly] private readonly List<int> _trialsToShuffle = new List<int> {2,3,4,5,6,7,8,9,10};
    
    private List<RouteFrame> _routeFrames;
    private List<List<GridElement>> _smoothPursuitRoutes;
    private List<List<GridElement>> _randomizedSmoothPursuitRoutes;
    private List<List<FreeViewingDataFrame>> _randomizedPictureList;
    
    private int _numberOfBlocks = 7;
    
    private void Start()
    {
        _random = new Random();
        _routeGenerator = GetComponent<RouteGenerator>();
        
        _routeFrames = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListLargeGrid");

        foreach (var route in _routeFrames)
        {
            _smoothPursuitRoutes.Add(route.Route);
        }

        _randomizedPictureList = RandomizeFreeViewingPictures();
        _randomizedSmoothPursuitRoutes = RandomizeSmoothPursuitSequence();
        
        GenerateBlocks();
    }

    private void GenerateBlocks(int numberOfBlocks = 7)
    {
        List<Block> listOfBlocks = new List<Block>();

        for (int i = 0; i < numberOfBlocks; i++)
        {
            listOfBlocks.Add(GenerateBlock(_randomizedPictureList[i], _randomizedSmoothPursuitRoutes[i]));
        }
        
        DataSavingManager.Instance.SaveList(listOfBlocks, "Blocks");
    }
    
    private Block GenerateBlock(List<FreeViewingDataFrame> freeViewingDataFrames, List<GridElement> smoothPursuit)
    {
        Block block = new Block
            
        {
            SequenceOfTrials = RandomizeTrials(),
            
            LargeGridClose = _routeGenerator.GetGridRoute(),
            LargeGridFar = _routeGenerator.GetGridRoute(),
            SmallGrid = _routeGenerator.GetGridRoute(),
            
            SmoothPursuit = smoothPursuit,
            
            // Roll = 
            // Yaw =
            // Pitch = 
            
            // Blink = 
            
            PupilDilation = _routeGenerator.RandomisePupilDilationDataFrame(),
            PupilDilationBlackFixationDuration = 7f + RandomUnity.Range(-.25f, .25f),
            
            FreeViewingPictureList = freeViewingDataFrames,
            
            // todo fill out the rest
        };
        
        
        
        return block;
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
    
    private List<List<FreeViewingDataFrame>> RandomizeFreeViewingPictures()
    {
        List<List<FreeViewingDataFrame>> list = new List<List<FreeViewingDataFrame>>();

        for (int i = 0; i < _numberOfBlocks; i++)
        {
            List<FreeViewingDataFrame> dataFrames = new List<FreeViewingDataFrame>();
            
            for (int j = 0; j < 3; j++)
            {
                int index = _random.Next(freeViewingPictures.Count);
                float jitter = RandomUnity.Range(-.2f, .2f);

                dataFrames[j].ObjectName = freeViewingPictures[index].name;
                dataFrames[j].Position = freeViewingPictures[index].transform.position;
                dataFrames[j].PhotoFixationDuration = 6;
                dataFrames[j].FixationPointDuration = .9f + jitter;
                dataFrames[j].Picture = freeViewingPictures[index];
                
                freeViewingPictures.RemoveAt(index);
            }
            
            list.Add(dataFrames);
        }
        
        return list;
    }
    
    private List<List<GridElement>> RandomizeSmoothPursuitSequence()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < _smoothPursuitRoutes.Count; i++)
        {
            int index = _random.Next(_smoothPursuitRoutes.Count);
            list.Add(_smoothPursuitRoutes[index]);
            _smoothPursuitRoutes.RemoveAt(index);
        }
        
        return list;
    }
}
