using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = System.Random;

public class ExperimentManager : MonoBehaviour
{
    #region Fields

    public static ExperimentManager Instance { get ; private set; } 
    
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject largeGrid1;
    [SerializeField] private GameObject largeGrid2;
    [SerializeField] private GameObject smallGrid;
    [SerializeField] private List<GameObject> freeViewingPictures;

    private List<List<GridElement>> _smoothPursuitRoutes;
    private List<List<GridElement>> _randimizedSmoothPursuitRoutes;
    private List<GameObject> _randomizedPictureList;
    private List<Block> _blocks;

    private Random _random;
    private bool _continueTrials;

    #endregion


    #region PrivateMethods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // todo read all of the randomization lists from the list
        
        _random = new Random();
        
        // todo read the routes from file
        _smoothPursuitRoutes = new List<List<GridElement>>();
        
        _randomizedPictureList = RandomizeFreeViewingPictures();
        _randimizedSmoothPursuitRoutes = RandomizeSmoothPursuitSequence();
        
        _blocks = new List<Block>();

        for (int i = 0; i < 6; i++)
        {
            _blocks.Add(GetComponent<BlockGenerator>().GenerateBlock(_randomizedPictureList[i], _smoothPursuitRoutes[i]));
        }
        
        // todo show instruction and welcome message
    }

    private void Update()
    {
        // TODO click or push trigger to start the experiment


        #region DebugingPurpose

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

        #endregion
        
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

    private List<GameObject> RandomizeFreeViewingPictures()
    {
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < freeViewingPictures.Count+1; i++)
        {
            int index = _random.Next(freeViewingPictures.Count);
            list.Add(freeViewingPictures[index]);
            freeViewingPictures.RemoveAt(index);
        }
        
        return list;
    }
    
    private List<List<GridElement>> RandomizeSmoothPursuitSequence()
    {
        List<List<GridElement>> list = new List<List<GridElement>>();

        for (int i = 0; i < _smoothPursuitRoutes.Count+1; i++)
        {
            int index = _random.Next(_smoothPursuitRoutes.Count);
            list.Add(_smoothPursuitRoutes[index]);
            _smoothPursuitRoutes.RemoveAt(index);
        }
        
        return list;
    }

    #endregion
    
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