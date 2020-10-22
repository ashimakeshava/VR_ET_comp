using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;
using Varjo;


public class ExperimentManager : MonoBehaviour
{
    #region Fields

    public static ExperimentManager Instance { get ; private set; }

    [SerializeField] private string participantId;
    
    [Space] [Header("Grids and Fixation Point")] 
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject largeGrid;
    
    [SerializeField] private Material mainSkyBox;
    
    [Space] [Header("Instructions")] 
    [SerializeField] private TextMesh welcome;    // todo write the welcome message and give instruction for calibration
    [SerializeField] private TextMesh blockEnd;    // todo write the block ended message
    [SerializeField] private TextMesh afterBlockThree;    // todo write the force break message
    [SerializeField] private TextMesh thankYou;    // todo write the experiment ended message
    [SerializeField] private List<TextMesh> trialInstructions;    // todo edit message
    
    private List<Block> _blocks;

    private bool _continue;
    private bool _trialIsRunning;
    private bool _welcomeState;
    private bool _endOfBlockState;
    private bool _endOfExperiment;
    private bool _inCalibration;
    private bool _dataSaved;
    private bool _participantIdAdded;
    
    private int _blockIndex;
    private int _trialIndex;

    // Data saving variables
    private string _trialName;
    
    private bool _fixationPointActivationState;
    private Vector3 _fixationPointPosition;
    
    private bool _globalFixationPointActivationState;
    private Vector3 _globalFixationPointPosition;
    
    private bool _stimuliOnset;
    
    private bool _headMovementStimuliActivationState;
    private string _headMovementObjectName;
    
    private bool _spacePressed;
    
    private bool _trialStarted;
    
    private string _contrastVariationName;


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
        _blocks = new List<Block>();

        ResetFixationPoint();

        GetComponent<Blink>().NotifyStimuliObservers += SetBlinkStimuliOnset;
    }

    private void OnApplicationQuit()        // Alt + f4 also works
    {
        SaveDataManually();
    }

    private void Update()
    {
        if (_participantIdAdded)
        {
            SetSpacePressedStatus(Input.GetKeyDown(KeyCode.Space));

            if (_inCalibration)
            {
                if (EyetrackingManager.Instance.IsCalibrated())
                {
                    _inCalibration = false;
                    EyetrackingManager.Instance.StartRecording();
                    TrialEnded();
                }
            }
        
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
                ResetStimuliDataFrame();
            
                if (_trialIndex < 12)
                {
                    TrialInstructionActivation(true);
                    if (_continue) ExecuteTrials();
                } 
                else if (_endOfBlockState)
                {
                    if (!_dataSaved)
                    {
                        SaveData();
                    }
                    
                    if (_blockIndex == 2) afterBlockThree.gameObject.SetActive(true);
                    else if (_blockIndex > 4)
                    {
                        thankYou.gameObject.SetActive(true);
                        _endOfExperiment = true;
                    }
                    else blockEnd.gameObject.SetActive(true);

                    if (!_endOfExperiment)
                    {
                        if (_continue)
                        {
                            GetComponent<StimuliDataRecorder>().StartStimuliDataRecording();
                            EyetrackingManager.Instance.StartRecording();
                            _dataSaved = false;
                        
                            _endOfBlockState = false;
                    
                            _blockIndex++;
                            _trialIndex = 0;
                            _continue = false;
                    
                            afterBlockThree.gameObject.SetActive(false);
                            blockEnd.gameObject.SetActive(false);
                            TrialInstructionActivation(true);
                            if (_continue) ExecuteTrials();
                        }
                    }
                }
                else
                {
                    _endOfBlockState = true;
                    _continue = false;
                }
            
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _continue = true;
                }
            }
        }
    }

    public void HeadCalibrationEnded()
    {
        StartExperiment();
    }
    
    private void StartExperiment()
    {
        _blocks = DataSavingManager.Instance.LoadFileList<Block>(participantId + "_Blocks_Varjo");    // todo handle this name as input

        _welcomeState = true;
        welcome.gameObject.SetActive(true);

        GetComponent<StimuliDataRecorder>().StartStimuliDataRecording();
    }
    
    private void SaveData()
    {
        string blockNum = (_blockIndex + 1).ToString();
                
        GetComponent<StimuliDataRecorder>().StopStimuliDataRecording(participantId, blockNum);
        EyetrackingManager.Instance.StopRecording(participantId, blockNum);

        _dataSaved = true;
    }
    
    private void SaveDataManually()
    {
        string incompleteData = "Incomplete";
                
        GetComponent<StimuliDataRecorder>().StopStimuliDataRecording(participantId, incompleteData);
        EyetrackingManager.Instance.StopRecording(participantId, incompleteData);

        _dataSaved = true;
    }

    private void ResetFixationPoint()
    {
        fixationPoint.transform.position = Vector3.zero;
        fixationPoint.gameObject.SetActive(false);
        SetFixationPointActivationStatus(false);
    }

    private void ResetStimuliDataFrame()
    {
        SetTrialName("None");
        SetFixationPointPosition(Vector3.zero);
        SetGlobalFixationPointPosition(Vector3.zero);
        SetHeadMovementObjectName("None");
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
        SetTrialActivationStatus(true);

        switch (_blocks[_blockIndex].SequenceOfTrials[_trialIndex])
        {
            case 0:    // Calibration
                _trials = Trials.Calibration;
                SetTrialName("Calibration");

                EyetrackingManager.Instance.StartCalibration();
                _inCalibration = true;

                break;
            case 1:    // Validation
                _trials = Trials.Validation;
                SetTrialName("Validation");

                GetComponent<Validation>().RunValidation(_blocks[_blockIndex].LargeGridClose, _blocks[_blockIndex].LargeGridFar);

                break;
            case 2:    // Smooth pursuit
                _trials = Trials.SmoothPursuit;
                SetTrialName("SmoothPursuit");

                GetComponent<SmoothPursuit>().RunSmoothPursuit(_blocks[_blockIndex].SmoothPursuit);

                break;
            case 3:    // Small grid
                _trials = Trials.SmallGrid;
                SetTrialName("SmallGrid");

                GetComponent<SmallGrid>().RunSmallGrid(_blocks[_blockIndex].SmallGrid);

                break;
            case 4:    // Blink
                _trials = Trials.Blink;
                SetTrialName("Blink");

                GetComponent<Blink>().RunBeepBlink(_blocks[_blockIndex].Blink);

                break;
            case 5:    // Pupil dilation
                _trials = Trials.PupilDilation;
                SetTrialName("PupilDilation");

                GetComponent<PupilDilation>().RunPupilDilation(_blocks[_blockIndex].PupilDilation, _blocks[_blockIndex].PupilDilationBlackFixationDuration);

                break;
            case 6:    // Free viewing
                _trials = Trials.FreeViewing;
                SetTrialName("FreeViewing");

                GetComponent<FreeViewing>().RunFreeViewing(_blocks[_blockIndex].FreeViewingPictureList);

                break;
            case 7:    // Roll
                _trials = Trials.Roll;
                SetTrialName("Roll");

                GetComponent<HeadTrackingSpace>().RunRoll(_blocks[_blockIndex].Roll);

                break;
            case 8:    // Yaw
                _trials = Trials.Yaw;
                SetTrialName("Yaw");

                GetComponent<HeadTrackingSpace>().RunYaw(_blocks[_blockIndex].Yaw);

                break;
            case 9:    // Pitch
                _trials = Trials.Pitch;
                SetTrialName("Pitch");
                
                GetComponent<HeadTrackingSpace>().RunPitch(_blocks[_blockIndex].Pitch);

                break;
            case 10:    // Micro saccades
                _trials = Trials.MicroSaccades;
                SetTrialName("MicroSaccades");
                
                GetComponent<MicroSaccades>().RunMicroSaccades();

                break;
        }

        _trialIndex++;
    }

    private void SetBlinkStimuliOnset()
    {
        SetStimuliActivationStatus(true);
        StartCoroutine(Timer(.1f));
    }

    IEnumerator Timer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetStimuliActivationStatus(false);
    }

    #endregion
    
    #region Getter and Setters

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
    
    public void TrialEnded()
    {
        _trialIsRunning = false;
        SetTrialActivationStatus(false);
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

    #region DataSaving Setters and Getters
    
    public int GetTrialsID()
    {
        return _trialIndex;
    }
    
    public void SetTrialName(string name)
    {
        _trialName = name;
    }
    public string GetTrialsName()
    {
        return _trialName;
    }
    public void SetFixationPointActivationStatus(bool onsetStatus)
    {
        _fixationPointActivationState = onsetStatus;
    }
    
    public bool GetFixationPointActivationStatus()
    {
        return _fixationPointActivationState;
    }

    public void SetFixationPointPosition(Vector3 position)
    {
        _fixationPointPosition = position;
    }
    
    public Vector3 GetFixationPointPosition()
    {
        return _fixationPointPosition;
    }

    public void SetGlobalFixationPointPosition(Vector3 position)
    {
        _globalFixationPointPosition = position;
    }
    
    public Vector3 GetGlobalFixationPointPosition()
    {
        return _globalFixationPointPosition;
    }

    public void SetGlobalFixationPointActivationStatus(bool onsetStatus)
    {
        _globalFixationPointActivationState = onsetStatus;
    }
    
    public bool GetGlobalFixationPointActivationStatus()
    {
        return _globalFixationPointActivationState;
    }
    
    public void SetStimuliActivationStatus(bool onsetStatus)
    {
        _stimuliOnset = onsetStatus;
    }
    
    public bool GetStimuliActivationStatus()
    {
        return _stimuliOnset;
    }

    public void SetHeadMovementStimuliActivationStatus(bool onsetStatus)
    {
        _headMovementStimuliActivationState = onsetStatus;
    }
    
    public bool GetHeadMovementStimuliActivationStatus()
    {
        return _headMovementStimuliActivationState;
    }

    public void SetHeadMovementObjectName(string objectName)
    {
        _headMovementObjectName = objectName;
    }
    
    public string GetHeadMovementObjectName()
    {
        return _headMovementObjectName;
    }
    
    public void SetSpacePressedStatus(bool status)
    {
        _spacePressed = status;
    }
    
    public bool GetSpacePressedStatus()
    {
        return _spacePressed;
    }
    
    public void SetTrialActivationStatus(bool status)
    {
        _trialStarted = status;
    }
    
    public bool GetTrialActivationStatus()
    {
        return _trialStarted;
    }
    
    public void SetContrastVariationName(string name)
    {
        _contrastVariationName = name;
    }
    
    public string GetContrastVariationName()
    {
        return _contrastVariationName;
    }

    #endregion
    
    #endregion

    #region GUI

    void OnGUI()
    {
        if (!_participantIdAdded)
        {
            GUI.Label(new Rect(Screen.width/2f, Screen.height/2f - 30, 80, 20), "Participant ID");
            participantId = GUI.TextField(new Rect(Screen.width/2f, Screen.height/2f, 100, 20), participantId, 3);
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                DataSavingManager.Instance.SetParticipantID(participantId);
                GetComponent<HeadTrackingSpace>().CalibrateHead();
                _participantIdAdded = true;
            }
        }
    }

    #endregion
}
