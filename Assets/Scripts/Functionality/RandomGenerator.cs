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
    private Random _random;

    private List<List<FreeViewingDataFrame>> _randomizedPictureList;

    private List<RouteFrame> _routeFramesLargeGridClose, 
        _routeFramesLargeGridFar, 
        _routeFramesSmallGrid, 
        _routeFramesSmoothPursuit;
    
    private List<List<GridElement>> _largeGridCloseRoutes, 
        _largeGridFarRoutes, 
        _smallGridRoutes, 
        _smoothPursuitRoutes;

    private List<List<GridElement>> _randomizedSmoothPursuitHTC,
        _randomizedLargeGridCloseHTC,
        _randomizedLargeGridFarHTC,
        _randomizedSmallGridHTC;
    
    private List<List<GridElement>> _randomizedSmoothPursuitVarjo,
        _randomizedLargeGridCloseVarjo,
        _randomizedLargeGridFarVarjo,
        _randomizedSmallGridVarjo;
    
    
    private int _numberOfBlocks = 7;
    int counter = 1;
    
    private void Start()
    {
        _random = new Random();
        
        _largeGridCloseRoutes = new List<List<GridElement>>();
        _largeGridFarRoutes = new List<List<GridElement>>();
        _smallGridRoutes = new List<List<GridElement>>();
        _smoothPursuitRoutes = new List<List<GridElement>>();
        _randomizedSmoothPursuitHTC = new List<List<GridElement>>();
        _randomizedLargeGridCloseHTC = new List<List<GridElement>>();
        _randomizedLargeGridFarHTC = new List<List<GridElement>>();
        _randomizedSmallGridHTC = new List<List<GridElement>>();
        _randomizedSmoothPursuitVarjo = new List<List<GridElement>>();
        _randomizedLargeGridCloseVarjo = new List<List<GridElement>>();
        _randomizedLargeGridFarVarjo = new List<List<GridElement>>();
        _randomizedSmallGridVarjo = new List<List<GridElement>>();

        _routeFramesLargeGridClose = DataSavingManager.Instance.LoadFileList<RouteFrame>("Grid 1");
        _routeFramesLargeGridFar = DataSavingManager.Instance.LoadFileList<RouteFrame>("Grid 2");
        _routeFramesSmallGrid = DataSavingManager.Instance.LoadFileList<RouteFrame>("Grid 3");
        _routeFramesSmoothPursuit = DataSavingManager.Instance.LoadFileList<RouteFrame>("Grid 4");

        /*List<List<RouteFrame>> routeFrames = new List<List<RouteFrame>>    // todo for pooling 
        {
            _routeFramesLargeGridClose,
            _routeFramesLargeGridFar,
            _routeFramesSmallGrid,
            _routeFramesSmoothPursuit
        };

        foreach (var route in routeFrames)
        {
            List<RouteFrame> list = new List<RouteFrame>();

            for (int i = 0; i < 12; i++)
            {
                int index = _random.Next(route.Count);
                list.Add(route[index]);
            }
            
            DataSavingManager.Instance.SaveList(list, "Grid " + counter);
            counter++;
        }*/

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

        _randomizedSmoothPursuitHTC = RandomizeSmoothPursuit();
        _randomizedSmoothPursuitVarjo = RandomizeSmoothPursuit();
        
        _randomizedSmallGridHTC = RandomizeSmallGrid();
        _randomizedSmallGridVarjo = RandomizeSmallGrid();

        _randomizedLargeGridCloseHTC = RandomizeLargeGridClose();
        _randomizedLargeGridCloseVarjo = RandomizeLargeGridClose();

        _randomizedLargeGridFarHTC = RandomizeLargeGridFar();
        _randomizedLargeGridFarVarjo = RandomizeLargeGridFar();
        
        _randomizedPictureList = RandomizeFreeViewingPictures();
        
        // generate blocks for varjo
        GenerateBlocks(true);
        
        // generate blocks for HTC
        GenerateBlocks();
    }

    private void GenerateBlocks(bool varjo = false, int numberOfBlocks = 6)
    {
        _numberOfBlocks = numberOfBlocks;
        
        List<Block> listOfBlocks = new List<Block>();
        string deviceName = "";
        
        for (int i = 0; i < numberOfBlocks; i++)
        {
            if (varjo)
            {
                deviceName = "Varjo";
                listOfBlocks.Add(GenerateBlock(_randomizedPictureList[i], _randomizedSmoothPursuitVarjo[i], _randomizedLargeGridCloseVarjo[i],
                    _randomizedLargeGridFarVarjo[i], _randomizedSmallGridVarjo[i]));
            }
            else
            {
                deviceName = "HTC";
                listOfBlocks.Add(GenerateBlock(_randomizedPictureList[i], _randomizedSmoothPursuitHTC[i], _randomizedLargeGridCloseHTC[i],
                    _randomizedLargeGridFarHTC[i], _randomizedSmallGridHTC[i]));
            }
        }
        
        DataSavingManager.Instance.SaveList(listOfBlocks, "Blocks " + deviceName);
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
            
            Roll = RandomizeHeadMovement("Roll"),
            Yaw = RandomizeHeadMovement("Yaw"),
            Pitch = RandomizeHeadMovement("Pitch"),
            
            Blink = RandomDelayGeneratorForBlink(),
            
            PupilDilation = RandomisePupilDilationDataFrame(),
            PupilDilationBlackFixationDuration = 7f + jitter,
            
            FreeViewingPictureList = freeViewingDataFrames
        };

        return block;
    }

    private List<int> RandomizeTrials()
    {
        List<int> trialsToShuffle = new List<int> {2,3,4,5,6,7,8,9,10};
        List<int> list = new List<int> {0, 1};

        for (int i = 0; i < 9; i++)
        {
            int index = _random.Next(trialsToShuffle.Count);
            list.Add(trialsToShuffle[index]);
            trialsToShuffle.RemoveAt(index);
        }
        
        list.Add(1);

        return list;
    }
    
    private List<List<FreeViewingDataFrame>> RandomizeFreeViewingPictures()
    {
        List<int> freeViewingIndices = new List<int>
        {
            0,1,2,3,4,5,6,7,
            8,9,10,11,12,13,14,
            15,16,17,18,19,20,21
        };

        List<List<FreeViewingDataFrame>> list = new List<List<FreeViewingDataFrame>>();

        for (int i = 0; i < _numberOfBlocks; i++)
        {
            List<FreeViewingDataFrame> dataFrames = new List<FreeViewingDataFrame>();
            
            for (int j = 0; j < 3; j++)
            {
                FreeViewingDataFrame freeViewingDataFrame = new FreeViewingDataFrame();
                
                int index = _random.Next(freeViewingIndices.Count);
                float jitter = RandomUnity.Range(-.2f, .2f);

                freeViewingDataFrame.IndexofTheStimuli = freeViewingIndices[index];
                freeViewingDataFrame.StimuliDuration = 6;
                freeViewingDataFrame.FixationPointDuration = .9f + jitter;
                
                dataFrames.Add(freeViewingDataFrame);
                freeViewingIndices.RemoveAt(index);
            }
            
            list.Add(dataFrames);
        }
        
        return list;
    }
    
    private List<List<GridElement>> RandomizeLargeGridClose()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < 6; i++)
        {
            int index = _random.Next(_largeGridCloseRoutes.Count);
            list.Add(_largeGridCloseRoutes[index]);
            _largeGridCloseRoutes.RemoveAt(index);
        }
        
        return list;
    }
    
    private List<List<GridElement>> RandomizeLargeGridFar()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < 6; i++)
        {
            int index = _random.Next(_largeGridFarRoutes.Count);
            list.Add(_largeGridFarRoutes[index]);
            _largeGridFarRoutes.RemoveAt(index);
        }
        
        return list;
    }
    
    
    private List<List<GridElement>> RandomizeSmallGrid()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < 6; i++)
        {
            int index = _random.Next(_smallGridRoutes.Count);
            list.Add(_smallGridRoutes[index]);
            _smallGridRoutes.RemoveAt(index);
        }
        
        return list;
    }
    
    private List<List<GridElement>> RandomizeSmoothPursuit()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < 6; i++)
        {
            int index = _random.Next(_smoothPursuitRoutes.Count);

            foreach (var element in _smoothPursuitRoutes[index])
            {
                float jitter = RandomUnity.Range(-.2f, .2f);
                element.StimuliDuration += jitter;
                
                jitter = RandomUnity.Range(-.2f, .2f);
                element.MovementDuration += jitter;
            }
            
            list.Add(_smoothPursuitRoutes[index]);
            _smoothPursuitRoutes.RemoveAt(index);
        }
        
        return list;
    }

    private List<PupilDilationDataFrame> RandomisePupilDilationDataFrame()
    {
        List<int> pupilDilationSequence = new List<int> {0,1,2,3};
        List<PupilDilationDataFrame> pupilDilationDataFrames = new List<PupilDilationDataFrame>();
        
        for (int i = 0; i < 4; i++)
        {
            PupilDilationDataFrame pupilDilationDataFrame = new PupilDilationDataFrame();
            
            int index = _random.Next(pupilDilationSequence.Count);
            float jitter = RandomUnity.Range(-.2f, .2f);
            
            pupilDilationDataFrame.StimuliIndex = index;
            pupilDilationDataFrame.StimuliDuration = 3f + jitter;

            pupilDilationDataFrames.Add(pupilDilationDataFrame);
            pupilDilationSequence.RemoveAt(index);
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

    private HeadMovement RandomizeHeadMovement(string movementType)
    {
        List<int> positions = new List<int> {
            0,1,2,3,4,
            0,1,2,3,4,
            0,1,2,3,4};
        
        HeadMovement movement = new HeadMovement {MovementType = movementType};
        List<int> position = new List<int>();
        List<float> delay = new List<float>();
        
        for (int i = 0; i < 15; i++)
        {
            int index = _random.Next(positions.Count);
            float duration = (RandomUnity.value <= 0.5) ? 1 : 1.5f;

            position.Add(positions[index]);
            delay.Add(duration);
            positions.RemoveAt(index);
        }
        
        movement.StimuliIndex = position;
        movement.DelayBeforeStimuli = delay;

        return movement;
    }
}
