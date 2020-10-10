using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = System.Random;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager Instance { get ; private set; } 
    
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject largeGrid1;
    [SerializeField] private GameObject largeGrid2;
    [SerializeField] private GameObject smallGrid;
    
    private List<Block> _blocks;

    private bool _continueTrials;
    
    private enum Trials
    {
        Validation = 1,
        SmoothPursuit = 2,
        SmallGrid = 3,
        Blink = 4,
        PupilDilation = 5,
        FreeViewing = 6,
        Roll = 7,
        Yaw = 8,
        Pitch = 9,
        MicroSaccades = 10
    }

    private Trials _trial;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _blocks = new List<Block>();

        for (int i = 0; i < 6; i++)
        {
            _blocks.Add(GetComponent<BlockGenerator>().GenerateBlock());
        }
        
        // TODO click to start the experiment
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<RouteGenerator>().GenerateGridRoute(smallGrid);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < smallGrid.transform.childCount;i++)
            {
                smallGrid.transform.GetChild(i).gameObject.SetActive(true);

            }
            fixationPoint.transform.position= new Vector3(0,0,1);
            Debug.Log("___________________________________-----_____________________________");
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartExperiment();
        }
    }


    void StartExperiment()
    {
        // todo iterate through blocks   
        foreach (var block in _blocks)
        {
            foreach (var value in block.SequenceOfTrials)
            {
                _continueTrials = false;
                ExecuteTrials(value, block);
                while (!_continueTrials) {}
            }
        }
    }

    
    // todo implement 
    void ExecuteTrials(int value, Block block)
    {
        switch (value)
        {
            case 1:
                GetComponent<Validation>().StartValidation(block.LargeGridClose, block.LargeGridFar);
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            case 4:
                
                break;
            case 5:
                
                break;
            case 6:
                
                break;
            case 7:
                
                break;
            case 8:
                
                break;
            case 9:
                
                break;
            case 10:
                
                break;
        }
    }

    #region Getter and Setters

    public void TrialEnded()
    {
        _continueTrials = true;
    }
    
    public GameObject GetFixationPoint()
    {
        return fixationPoint;
    }
    
    public GameObject GetLargeGrid1()
    {
        return largeGrid1;
    }
    
    public GameObject GetLargeGrid2()
    {
        return largeGrid2;
    }
    public GameObject GetSmallGrid()
    {
        return smallGrid;
    }

    #endregion
}

        
    // TODO implement movement
    // TODO smoothPursuit has too few elements
    // TODO read from the file to go on with the movement 