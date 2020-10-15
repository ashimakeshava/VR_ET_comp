using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;
using RandomSystem = System.Random;

public class HeadTrackingSpace : MonoBehaviour
{
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private float countdownUntilAligned = 5f;
    
    public GameObject orientationCross;
    public GameObject fixationCrossObject;
    public GameObject yawSetup;
    public GameObject pitchSetup;
    public GameObject rollSetup;

    private bool _calibrationStatus;
    private bool _experimentStatus;
    private bool _isReadyToGo;
    private bool _isInResetStatus;
    private bool _isPeriodicallyResetting;
    private bool _trialEnded;
    
    private bool _isYaw;
    private bool _isPitch;
    private bool _isRoll;

    private RandomSystem _random;
    private FixationCross _fixationCross;
    private HeadMovement _yawMovement, _pitchMovement, _rollMovement;
    
    private float _counter;

    
    void Start()
    {
        _random = new RandomSystem();
        
        SetStandByStatus();
        
        _fixationCross = fixationCrossObject.GetComponent<FixationCross>();
        _yawMovement = new HeadMovement();
        _pitchMovement = new HeadMovement();
        _rollMovement = new HeadMovement();

        _counter = countdownUntilAligned;
        _isPeriodicallyResetting = false;
        _isInResetStatus = false;
    }

    void Update()
    {
        if (_calibrationStatus)
        {
            if (!_isPeriodicallyResetting)
            {
                StartCoroutine(PeriodicallyResetCameraPosition(2f));
            }
            
            if (_fixationCross.GetAlignment())
            {
                _counter-= Time.deltaTime;
            }
            else
            {
                _counter= 3f;
                SetCalibrationStatus();
            }
            if (_counter <= 0f)
            {
                SetExperimentStatus();
            }
        }

        if (_experimentStatus)
        {
            if (_isYaw)
            {
                if (!_yawMovement.MovementPosition.Any())
                {
                    SetStandByStatus();
                    _isReadyToGo = false;
                    _experimentStatus = false;
                    ExperimentManager.Instance.TrialEnded();
                }
                
                if (_isReadyToGo)
                {
                    int index = _random.Next(_yawMovement.MovementPosition.Count);

                    StartCoroutine(StartYaw(_yawMovement.MovementPosition[index], _yawMovement.DelayBeforeStimuli[index]));
                    
                    _yawMovement.MovementPosition.RemoveAt(index);
                    _yawMovement.DelayBeforeStimuli.RemoveAt(index);
                    
                    _isReadyToGo = false;
                }
                
                if (Input.GetKeyDown(KeyCode.Space) && _fixationCross.GetAlignment())
                {
                    SetStandByStatus();
                    _isReadyToGo = true;
                }
            }

            if (_isPitch)
            {
                if (!_pitchMovement.MovementPosition.Any())
                {
                    SetStandByStatus();
                    _isReadyToGo = false;
                    _experimentStatus = false;
                    ExperimentManager.Instance.TrialEnded();
                }
                
                if (_isReadyToGo)
                {
                    int index = _random.Next(_pitchMovement.MovementPosition.Count);
                    
                    StartCoroutine(StartPitch(_pitchMovement.MovementPosition[index], _pitchMovement.DelayBeforeStimuli[index]));
                    
                    _pitchMovement.MovementPosition.RemoveAt(index);
                    _pitchMovement.DelayBeforeStimuli.RemoveAt(index);

                    _isReadyToGo = false;
                }
                
                if (Input.GetKeyDown(KeyCode.Space) && _fixationCross.GetAlignment())
                {
                    SetStandByStatus();
                    _isReadyToGo = true;
                }
            }

            if (_isRoll)
            {
                if (!_rollMovement.MovementPosition.Any())
                {
                    SetStandByStatus();
                    _isReadyToGo = false;
                    _experimentStatus = false;
                    ExperimentManager.Instance.TrialEnded();
                }

                if (_isReadyToGo)
                {
                    int index = _random.Next(_rollMovement.MovementPosition.Count);

                    StartCoroutine(StartRoll(_rollMovement.MovementPosition[index], _rollMovement.DelayBeforeStimuli[index]));
                    
                    _rollMovement.MovementPosition.RemoveAt(index);
                    _isReadyToGo = false;
                }
                
                if (Input.GetKeyDown(KeyCode.Space) && _fixationCross.GetAlignment())
                {
                    SetStandByStatus();
                    _isReadyToGo = true;
                }
            }
        }
    }

    private void SetStandByStatus()
    {
       fixationPoint.SetActive(false);
       fixationCrossObject.SetActive(false);
       orientationCross.SetActive(false);
    }

    private IEnumerator StartPitch(int objectIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        fixationPoint.SetActive(true);
        fixationCrossObject.SetActive(true);
        
        _fixationCross.DisableVertical();
        pitchSetup.SetActive(true);
        rollSetup.SetActive(false);
        yawSetup.SetActive(false);
        
        foreach (Transform pos in pitchSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }
        
        SetPitchPositionActive(objectIndex);
        _fixationCross.SetTargetObject(pitchSetup.transform.GetChild(objectIndex).gameObject);
    }

    private IEnumerator StartYaw(int objectIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        fixationPoint.SetActive(true);
        fixationCrossObject.SetActive(true);

        _fixationCross.DisableHorizontal();
        pitchSetup.SetActive(false);
        rollSetup.SetActive(false);
        yawSetup.SetActive(true);
        
        foreach (Transform pos in yawSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }
        
        SetYawPositionActive(objectIndex);
        _fixationCross.SetTargetObject(yawSetup.transform.GetChild(objectIndex).gameObject);
    }

    private IEnumerator StartRoll(int objectIndex, float delay)
    {
        yield return new WaitForSeconds(delay);

        fixationPoint.SetActive(true);
        fixationCrossObject.SetActive(true);

        _fixationCross.DisableVertical();
        pitchSetup.SetActive(false);
        rollSetup.SetActive(true);
        yawSetup.SetActive(false);
        
        foreach (Transform pos in rollSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }

        SetRollPositionActive(objectIndex);
        _fixationCross.SetTargetObject(rollSetup.transform.GetChild(objectIndex).gameObject);
    }

    private void ResetCameraPosition()
    {
        //Todd Wassen miracle solution for aligning the WorldSpace to the Camera
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
        
        fixationPoint.transform.position = new Vector3(orientationCross.transform.position.x,orientationCross.transform.position.y, orientationCross.transform.position.z);
    }
    
    private void SetCalibrationStatus()
    {
        orientationCross.SetActive(true);
        fixationCrossObject.SetActive(true);
        fixationPoint.SetActive(false);
        foreach (Transform child in orientationCross.transform)
        {
            child.transform.gameObject.SetActive(true);
        }

        _experimentStatus = false;
        _calibrationStatus = true;
        _isReadyToGo = false;
    }
    
    
    IEnumerator MakeReady(float sec)
    {
        if (!_isInResetStatus)
        {
            _isInResetStatus = true;
            
            yield return new WaitForSeconds(sec);
            
            _isInResetStatus = false;
        }
    }
    
    
    
    IEnumerator PeriodicallyResetCameraPosition(float sec)
    {
        _isPeriodicallyResetting=true;
        
        while (!_fixationCross.GetAlignment())
        {
            // Debug.Log("periodic allignment");
            yield return new WaitForSeconds(sec);
            ResetCameraPosition();
        }

        _isPeriodicallyResetting = false;

    }
    

    private void SetExperimentStatus()
    {
        orientationCross.SetActive(false);
        fixationPoint.SetActive(true);

        foreach (Transform child in orientationCross.transform)
        {
            child.transform.gameObject.SetActive(false);
        }

        _counter = countdownUntilAligned;

        _calibrationStatus = false;
        _experimentStatus = true;
        _isReadyToGo = true;
    }



    private void SetYawPositionActive(int i)
    {
        yawSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }

    private void SetPitchPositionActive(int i)
    {
        pitchSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }
    
    private void SetRollPositionActive(int i)
    {
        rollSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }


    public void RunYaw(HeadMovement yawMovement)
    {
        _yawMovement = yawMovement;
        _isYaw = true;
        _isRoll = false;
        _isPitch = false;

        _calibrationStatus = true;
    }
    
    public void RunPitch(HeadMovement pitchMovement)
    {
        _pitchMovement = pitchMovement;
        _isYaw = false;
        _isRoll = false;
        _isPitch = true;
        
        _calibrationStatus = true;
    }
    
    public void RunRoll(HeadMovement rollMovement)
    {
        _rollMovement = rollMovement;
        _isYaw = false;
        _isRoll = true;
        _isPitch = false;
        
        _calibrationStatus = true;
    }
}
