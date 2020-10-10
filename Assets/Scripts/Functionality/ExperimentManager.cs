using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using Random = System.Random;

public class ExperimentManager : MonoBehaviour
{
    #region Fields

    public static ExperimentManager Instance { get ; private set; } 
    
    [Header("Grids and Fixation Point")] [Space]
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject largeGrid;
    [SerializeField] private GameObject smallGrid;    // todo remove
    
    [Header("Constant Trials between all Participants")] [Space]
    [SerializeField] private List<GameObject> freeViewingPictures;
    [SerializeField] private List<Text> trialInstructions;
    
    [Header("Texts")] [Space]
    [SerializeField] private Text welcome;    // todo write the welcome message and give instruction for calibration
    [SerializeField] private Text blockEnd;    // todo write the block ended message
    [SerializeField] private Text afterBlockThree;    // todo write the force break message
    [SerializeField] private Text thankYou;    // todo write the experiment ended message

    private List<List<GridElement>> _smoothPursuitRoutes;
    private List<List<GridElement>> _randomizedSmoothPursuitRoutes;
    private List<GameObject> _randomizedPictureList;
    private List<Block> _blocks;

    private Random _random;
    private bool _continue;
    private bool _trialIsRunning;
    private int _blockIndex;
    private int _trialIndex;

    #endregion


    #region PrivateMethods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        // todo read the corresponding file or make one if !exists
    }

    private void Start()
    {
        _random = new Random();
        _blocks = new List<Block>();

        // todo read all of the randomization lists from the list
        _smoothPursuitRoutes = new List<List<GridElement>>();    // todo get routes from file
        
        _randomizedPictureList = RandomizeFreeViewingPictures();
        _randomizedSmoothPursuitRoutes = RandomizeSmoothPursuitSequence();
        
        
        for (int i = 0; i < 6; i++)
        {
            _blocks.Add(GetComponent<BlockGenerator>().GenerateBlock(_randomizedPictureList[i], _smoothPursuitRoutes[i]));
            // todo save the data
        }
        
        welcome.gameObject.SetActive(true);
    }

    private void Update()
    {
        // todo click or push trigger to continue with the experiment
        if (!_trialIsRunning)
        {
            ResetFixationPoint();
            
            if (_blockIndex == 6)
            {
                thankYou.gameObject.SetActive(true);
            }
            else if (_trialIndex == 12)
            {
                // todo save data
                if (_blockIndex == 3) afterBlockThree.gameObject.SetActive(true);
                else blockEnd.gameObject.SetActive(true);

                _blockIndex++;
                _trialIndex = 0;

                if (_continue)
                {
                    _continue = false;
                    afterBlockThree.gameObject.SetActive(false);
                    blockEnd.gameObject.SetActive(false);
                    TrialInstructionActivation(true);
                    if (_continue) ExecuteTrials();
                }
            }
            else
            {
                TrialInstructionActivation(true);
                if (_continue) ExecuteTrials();
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _continue = true;
            }
        }

        

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
    }

    private void ResetFixationPoint()
    {
        fixationPoint.transform.position = new Vector3(0, 0, 1);
        fixationPoint.gameObject.SetActive(false);
    }


    private void TrialInstructionActivation(bool activate)
    {
        welcome.gameObject.SetActive(false);
        trialInstructions[_blocks[_blockIndex].SequenceOfTrials[_trialIndex]].gameObject.SetActive(activate);
    }
    
    
    void ExecuteTrials()    // todo implement 
    {
        _trialIsRunning = true;
        _continue = false;
        TrialInstructionActivation(false);
        
        switch (_blocks[_blockIndex].SequenceOfTrials[_trialIndex])
        {
            case 0:    // calibration
                // calibration
                break;
            case 1:    // Validation
                GetComponent<Validation>().StartValidation(_blocks[_blockIndex].LargeGridClose, _blocks[_blockIndex].LargeGridFar);
                break;
            case 2:    // Smooth pursuit
                
                break;
            case 3:    // Small grid
                
                break;
            case 4:    // Blink
                
                break;
            case 5:    // Pupil dilation
                
                break;
            case 6:    // Free viewing
                
                break;
            case 7:    // Roll
                
                break;
            case 8:    // Yaw
                
                break;
            case 9:    // Pitch
                
                break;
            case 10:    // Micro saccades
                GetComponent<MicroSaccades>().RunMicroSaccades();
                break;
        }

        _trialIndex++;
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
        _trialIsRunning = false;
    }

    public GameObject GetFixationPoint()
    {
        return fixationPoint;
    }
    
    public GameObject GetLargeGrid()
    {
        return largeGrid;
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