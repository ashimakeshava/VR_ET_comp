using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;


public class ExperimentManager : MonoBehaviour
{
    #region Fields

    public static ExperimentManager Instance { get ; private set; } 
    
    [Space] [Header("Grids and Fixation Point")] 
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject largeGrid;
    [SerializeField] private GameObject smallGrid;    // todo remove
    
    [Space] [Header("Instructions")] 
    [SerializeField] private Text welcome;    // todo write the welcome message and give instruction for calibration
    [SerializeField] private Text blockEnd;    // todo write the block ended message
    [SerializeField] private Text afterBlockThree;    // todo write the force break message
    [SerializeField] private Text thankYou;    // todo write the experiment ended message
    [SerializeField] private List<Text> trialInstructions;    // todo edit message
    
    private List<Block> _blocks;

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
        
        _blocks = new List<Block>();
        _blocks = DataSavingManager.Instance.LoadFileList<Block>("Block");    // todo handle this name as input
        // todo load sheet
    }
    

    private void Start()
    {
        welcome.gameObject.SetActive(true);
    }

    private void Update()
    {
        // todo click or push trigger to continue with the experiment
        if (!_trialIsRunning)
        {
            ResetFixationPoint();
            
            if (_blockIndex == 6)    // todo 7 in case of the trial
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
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _continue = true;
            }
        }
    }

    private void ResetFixationPoint()
    {
        fixationPoint.transform.position = Vector3.forward;
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
                // todo call calibration
                break;
            case 1:    // Validation
                GetComponent<Validation>().RunValidation(_blocks[_blockIndex].LargeGridClose, _blocks[_blockIndex].LargeGridFar);
                break;
            case 2:    // Smooth pursuit
                GetComponent<SmoothPursuit>().RunSmoothPursuit(_blocks[_blockIndex].SmoothPursuit);
                break;
            case 3:    // Small grid
                GetComponent<SmallGrid>().RunSmallGrid(_blocks[_blockIndex].SmallGrid);
                break;
            case 4:    // Blink
                GetComponent<Blink>().RunBeepBlink(_blocks[_blockIndex].Blink);
                break;
            case 5:    // Pupil dilation
                GetComponent<PupilDilation>().RunPupilDilation(_blocks[_blockIndex].PupilDilation, _blocks[_blockIndex].PupilDilationBlackFixationDuration);
                break;
            case 6:    // Free viewing
                GetComponent<FreeViewing>().RunFreeViewing(_blocks[_blockIndex].FreeViewingPictureList);
                break;
            case 7:    // Roll
                GetComponent<HeadTrackingSpace>().RunRoll(_blocks[_blockIndex].Roll);
                break;
            case 8:    // Yaw
                GetComponent<HeadTrackingSpace>().RunYaw(_blocks[_blockIndex].Yaw);
                break;
            case 9:    // Pitch
                GetComponent<HeadTrackingSpace>().RunPitch(_blocks[_blockIndex].Pitch);
                break;
            case 10:    // Micro saccades
                GetComponent<MicroSaccades>().RunMicroSaccades();
                break;
        }

        _trialIndex++;
    }

    public GameObject GetCurrentActiveGrid()
    {
        var GridList = GetGridList();
        GameObject activeGrid=new GameObject();
        bool found=false;
        foreach (var grid in GridList)
        {
            if (grid.activeInHierarchy&& !found)
            {
                activeGrid = grid;
                found = true;
            }

            if (grid.activeInHierarchy && found)
            {
                Debug.LogWarning("Two Grids are active, this can cause problems, deactivate one");
            }
        }
        return activeGrid;
    }

    private List<GameObject> GetGridList()
    {
        List<GameObject> GridList = new List<GameObject>();
        foreach (Transform child in mainCamera.transform)
        {
            if (child.gameObject != fixationPoint)
                GridList.Add(child.gameObject);
        }

        return GridList;
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

    // TODO implement the trial
    // TODO implement movement
    // TODO smoothPursuit has too few elements
    // TODO read from the file to go on with the movement 