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
    
    [SerializeField] private Material mainSkyBox;
    
    [Space] [Header("Instructions")] 
    [SerializeField] private Text welcome;    // todo write the welcome message and give instruction for calibration
    [SerializeField] private Text blockEnd;    // todo write the block ended message
    [SerializeField] private Text afterBlockThree;    // todo write the force break message
    [SerializeField] private Text thankYou;    // todo write the experiment ended message
    [SerializeField] private List<Text> trialInstructions;    // todo edit message
    
    private List<Block> _blocks;

    private bool _continue;
    private bool _trialIsRunning;
    private bool _welcomeState;
    
    private int _blockIndex;
    private int _trialIndex;

    enum Trials
    {
        Calibration,
        Validation,
        SmoothPursuit,
        SmallGrid,
        Blink,
        PupilDilation,
        Roll,
        Yaw,
        Pitch,
        FreeViewing,
        MicroSaccades
    }

    private Trials _trials;
    
    #endregion


    #region PrivateMethods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        // todo load sheet?
    }
    
    private void Start()
    {
        _welcomeState = true;
        welcome.gameObject.SetActive(true);
        
        _blocks = new List<Block>();
        _blocks = DataSavingManager.Instance.LoadFileList<Block>("Blocks Varjo");    // todo handle this name as input
    }

    private void Update()
    {
        if (_welcomeState)
        {
            ResetFixationPoint();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                welcome.gameObject.SetActive(false);
                _welcomeState = false;
            }
        }
        else if (!_trialIsRunning)
        {
            ResetFixationPoint();
            
            if (_blockIndex == 6)    // todo 7 in case of the training
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
                    // todo start recording
                    
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
        trialInstructions[_blocks[_blockIndex].SequenceOfTrials[_trialIndex]].gameObject.SetActive(activate);
    }

    void ExecuteTrials()
    {
        _trialIsRunning = true;
        _continue = false;
        TrialInstructionActivation(false);

        switch (_blocks[_blockIndex].SequenceOfTrials[_trialIndex])
        {
            case 0:    // calibration
                // EyetrackingManager.Instance.StartCalibration(); // todo get the calibration up and running

                _trials = Trials.Calibration;
                Debug.Log("Eye-calibration");    // todo remove
                TrialEnded();    // todo remove
                
                break;
            case 1:    // Validation
                _trials = Trials.Validation;
                GetComponent<Validation>().RunValidation(_blocks[_blockIndex].LargeGridClose, _blocks[_blockIndex].LargeGridFar);
                break;
            case 2:    // Smooth pursuit
                _trials = Trials.SmoothPursuit;
                GetComponent<SmoothPursuit>().RunSmoothPursuit(_blocks[_blockIndex].SmoothPursuit);
                break;
            case 3:    // Small grid
                _trials = Trials.SmallGrid;
                GetComponent<SmallGrid>().RunSmallGrid(_blocks[_blockIndex].SmallGrid);
                break;
            case 4:    // Blink
                _trials = Trials.Blink;
                GetComponent<Blink>().RunBeepBlink(_blocks[_blockIndex].Blink);
                break;
            case 5:    // Pupil dilation
                _trials = Trials.PupilDilation;
                GetComponent<PupilDilation>().RunPupilDilation(_blocks[_blockIndex].PupilDilation, _blocks[_blockIndex].PupilDilationBlackFixationDuration);
                break;
            case 6:    // Free viewing
                _trials = Trials.FreeViewing;
                GetComponent<FreeViewing>().RunFreeViewing(_blocks[_blockIndex].FreeViewingPictureList);
                break;
            case 7:    // Roll
                _trials = Trials.Roll;
                GetComponent<HeadTrackingSpace>().RunRoll(_blocks[_blockIndex].Roll);
                break;
            case 8:    // Yaw
                _trials = Trials.Yaw;
                GetComponent<HeadTrackingSpace>().RunYaw(_blocks[_blockIndex].Yaw);
                break;
            case 9:    // Pitch
                _trials = Trials.Pitch;
                GetComponent<HeadTrackingSpace>().RunPitch(_blocks[_blockIndex].Pitch);
                break;
            case 10:    // Micro saccades
                _trials = Trials.MicroSaccades;
                GetComponent<MicroSaccades>().RunMicroSaccades();
                break;
        }

        _trialIndex++;
    }

    public GameObject GetCurrentActiveGrid()
    {
        var gridList = GetGridList();
        GameObject activeGrid=new GameObject();
        bool found = false;
        foreach (var grid in gridList)
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
        List<GameObject> gridList = new List<GameObject>();
        foreach (Transform child in mainCamera.transform)
        {
            if (child.gameObject != fixationPoint)
                gridList.Add(child.gameObject);
        }

        return gridList;
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
    
    public GameObject GetGrid()
    {
        return largeGrid;
    }

    public Material GetMainSkybox()
    {
        return mainSkyBox;
    }
    
    #endregion
}
