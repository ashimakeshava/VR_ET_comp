using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;


public class RandomGenerator : MonoBehaviour
{
    [Header("Constant Trials between all Participants")] [Space]
    [SerializeField] private List<GameObject> freeViewingPictures;
    
    private Random _random;

    [ReadOnly] private readonly List<int> _trialsToShuffle = new List<int> {2,3,4,5,6,7,8,9,10};
    [ReadOnly] private readonly List<int> _pupilDilationSequence = new List<int> {0,1,2,3};
    [ReadOnly] private readonly List<int> _yawAndPitchMovements = new List<int> {0,1,2,3,4};
    [ReadOnly] private readonly List<int> _rollMovements = new List<int> {0,1,2,3,4,5};
    
    private List<RouteFrame> _routeFramesLargeGridClose, _routeFramesLargeGridFar, _routeFramesSmallGrid, _routeFramesSmoothPursuit;
    private List<List<GridElement>> _largeGridCloseRoutes, _largeGridFarRoutes, 
        _smallGridRoutes, _smoothPursuitRoutes, _randomizedSmoothPursuitRoutes;
    private List<List<FreeViewingDataFrame>> _randomizedPictureList;
    
    private int _numberOfBlocks = 7;
    
    private void Start()
    {
        _random = new Random();
        _largeGridCloseRoutes = _largeGridFarRoutes = _smallGridRoutes = _smoothPursuitRoutes = 
            _randomizedSmoothPursuitRoutes = new List<List<GridElement>>();
        
        _routeFramesLargeGridClose = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListLargeGrid1");
        _routeFramesLargeGridFar = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListLargeGrid2");
        _routeFramesSmallGrid = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListSmallGrid");
        _routeFramesSmoothPursuit = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListSmoothPursuit");

        foreach (var route in _routeFramesLargeGridClose)
        {
            _largeGridCloseRoutes.Add(route.Route);
        }
        
        foreach (var route in _routeFramesLargeGridFar)
        {
            _largeGridFarRoutes.Add(route.Route);
        }
        
        foreach (var route in _routeFramesSmallGrid)
        {
            _smallGridRoutes.Add(route.Route);
        }

        foreach (var route in _routeFramesSmoothPursuit)
        {
            _smoothPursuitRoutes.Add(route.Route);
        }

        _randomizedSmoothPursuitRoutes = RandomizeSmoothPursuitSequence();
        _randomizedPictureList = RandomizeFreeViewingPictures();
        
        GenerateBlocks();
    }

    private void GenerateBlocks(int numberOfBlocks = 7)
    {
        List<Block> listOfBlocks = new List<Block>();

        for (int i = 0; i < numberOfBlocks; i++)
        {
            listOfBlocks.Add(GenerateBlock(_randomizedPictureList[i], _randomizedSmoothPursuitRoutes[i], _largeGridCloseRoutes[i],
                _largeGridFarRoutes[i], _smallGridRoutes[i]));
        }
        
        DataSavingManager.Instance.SaveList(listOfBlocks, "Blocks");
    }
    
    private Block GenerateBlock(List<FreeViewingDataFrame> freeViewingDataFrames, List<GridElement> smoothPursuit, 
        List<GridElement> largeGrid1, List<GridElement> largeGrid2, List<GridElement> smallGrid)
    {
        float jitter = RandomUnity.Range(-.25f, .25f);
        Block block = new Block
            
        {
            SequenceOfTrials = RandomizeTrials(),
            
            LargeGridClose = largeGrid1,
            LargeGridFar = largeGrid2,
            SmallGrid = smallGrid,
            
            SmoothPursuit = smoothPursuit,
            
            Roll = RandomizeHeadMovement(6, "Roll"),
            Yaw = RandomizeHeadMovement(5, "Yaw"),
            Pitch = RandomizeHeadMovement(5, "Roll"),
            
            Blink = RandomDelayGeneratorForBlink(),
            
            PupilDilation = RandomisePupilDilationDataFrame(),
            PupilDilationBlackFixationDuration = 7f + jitter,
            
            FreeViewingPictureList = freeViewingDataFrames
        };

        return block;
    }

    private List<int> RandomizeTrials()
    {
        List<int> list = new List<int> {0, 1};

        for (int i = 0; i < _trialsToShuffle.Count; i++)
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

    private List<PupilDilationDataFrame> RandomisePupilDilationDataFrame()
    {
        List<PupilDilationDataFrame> pupilDilationDataFrames = new List<PupilDilationDataFrame>();
        
        for (int i = 0; i < _pupilDilationSequence.Count; i++)
        {
            int index = _random.Next(_pupilDilationSequence.Count);
            float jitter = RandomUnity.Range(-.2f, .2f);
            
            pupilDilationDataFrames[i].ColorIndex = index;
            pupilDilationDataFrames[i].ColorDuration = 3f + jitter;

            _pupilDilationSequence.RemoveAt(index);
        }

        return pupilDilationDataFrames;
    }

    private List<float> RandomDelayGeneratorForBlink()
    {
        List<float> delays = new List<float>();

        for (int i = 0; i < 11; i++)
        {
            float jitter = RandomUnity.Range(-.2f, .2f);
            delays.Add(1.5f + jitter);
        }

        return delays;
    }

    private HeadMovement RandomizeHeadMovement(int numberOfObjects, string movementType)
    {
        HeadMovement movement = new HeadMovement {MovementType = movementType};

        for (int i = 0; i < numberOfObjects; i++)
        {
            if (movementType == "Roll")
            {
                int index = _random.Next(_rollMovements.Count);
                movement.MovementPosition.Add(index);
            }
            else
            {
                int index = _random.Next(_yawAndPitchMovements.Count);
                movement.MovementPosition.Add(index);
            }
        }

        // todo do we add fixation time?
        return movement;
    }
}
