using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HeadTrackingSpace : MonoBehaviour
{
    public GameObject OrientationCross;

    public GameObject FixationCrossObject;

    public GameObject YawSetup;

    public GameObject PitchSetup;

    public GameObject RollSetup;

    private bool CalibrationStatus;

    private bool ExperimentStatus;

    private bool _isYaw;
    private bool _isPitch;
    private bool _isRoll;

    private bool _isReadyToGo;
    private bool isInResetStatus;

    private FixationCross fixationCross;

    [SerializeField] private GameObject FixationPoint;

    private HeadMovement _yawMovement, _pitchMovement, _rollMovement;

    
    [SerializeField] private float CountdownUntilAligned=5f;

    private float Counter;

    private bool isPeriodicallyResetting;
    // Start is called before the first frame update
    void Start()
    {
        // CalibrationStatus = true;
        fixationCross = FixationCrossObject.GetComponent<FixationCross>();
        _yawMovement = new HeadMovement();
        _pitchMovement = new HeadMovement();
        _rollMovement = new HeadMovement();

        Counter = CountdownUntilAligned;
        isPeriodicallyResetting = false;
        isInResetStatus = false;
        
        RunPitch(_pitchMovement);
    }

    // Update is called once per frame
    void Update()
    {
        if (CalibrationStatus)
        {
            if (!isPeriodicallyResetting)
            {
                StartCoroutine(PeriodicallyResetCameraPosition(2f));
            }
            
            if (fixationCross.GetAlignment())
            {
                Counter-= Time.deltaTime;
            }
            else
            {
                Counter= 3f;
                SetCalibrationStatus();
            }
            if (Counter <= 0f)
            {
                
                SetExperimentStatus();
            }
        }

        if (ExperimentStatus)
        {
            if (_isYaw)
            {
                if (_isReadyToGo)
                {
                    int index = Random.Range(0, 5);
                    StartYaw(index);
                    _isReadyToGo = false;
                }
                
                if (Input.GetKey(KeyCode.Space)&&fixationCross.GetAlignment())
                {
                    Debug.Log("finished");
                    SetStandybyStatus();
                    StartCoroutine(MakeReady(3));
                }
            }

            if (_isPitch)
            {
                if (_isReadyToGo)
                {
                    int index = Random.Range(0, 5);
                    StartPitch(index);
                    _isReadyToGo = false;
                }
                
                if (Input.GetKey(KeyCode.Space)&&fixationCross.GetAlignment())
                {
                    Debug.Log("finished");
                    SetStandybyStatus();
                    StartCoroutine(MakeReady(3));
                }
                
                //ExperimentStatus = _isYaw = _isRoll = _isPitch = false;
            }

            if (_isRoll)
            {
                if (_isReadyToGo)
                {
                    int index = Random.Range(0, 5);
                    StartRoll(index);
                    _isReadyToGo = false;
                }
                
                if (Input.GetKey(KeyCode.Space)&&fixationCross.GetAlignment())
                {
                    Debug.Log("finished");
                    SetStandybyStatus();
                    StartCoroutine(MakeReady(3));
                }
            }
        }
    }
    
    

    private void SetStandybyStatus()
    {
       FixationPoint.SetActive(false);
       FixationCrossObject.SetActive(false);
       OrientationCross.SetActive(false);
    }

    private void StartPitch(int index)
    {
        
        FixationPoint.SetActive(true);
        FixationCrossObject.SetActive(true);
        
        fixationCross.DisableVertical();
        PitchSetup.SetActive(true);
        RollSetup.SetActive(false);
        YawSetup.SetActive(false);
        
        foreach (Transform pos in PitchSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }
        
        

        
        SetPitchPositionActive(index);
        fixationCross.SetTargetObject(PitchSetup.transform.GetChild(index).gameObject);
    }

    private void StartYaw(int index)
    {
        FixationPoint.SetActive(true);
        FixationCrossObject.SetActive(true);

        fixationCross.DisableHorizontal();
        PitchSetup.SetActive(false);
        RollSetup.SetActive(false);
        YawSetup.SetActive(true);
        
        foreach (Transform pos in YawSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }
        SetYawPositionActive(index);
        fixationCross.SetTargetObject(YawSetup.transform.GetChild(index).gameObject);
    }

    private void StartRoll(int index)
    {
        FixationPoint.SetActive(true);
        FixationCrossObject.SetActive(true);

        fixationCross.DisableVertical();
        PitchSetup.SetActive(false);
        RollSetup.SetActive(true);
        YawSetup.SetActive(false);
        foreach (Transform pos in RollSetup.transform)
        {
            pos.gameObject.SetActive(false);
        }

        
        SetRollPositionActive(index);

        fixationCross.SetTargetObject(RollSetup.transform.GetChild(index).gameObject);
    }

    private void ResetCameraPosition()
    {
        //Todd Wassen miracle solution for alligning the WorldSpace to the Camera
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(
            Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);

        float yPos = Camera.main.transform.position.y;

        //YawSetup.transform.position = new Vector3(YawSetup.transform.position.x,yPos, YawSetup.transform.position.z);
       // PitchSetup.transform.position = new Vector3(PitchSetup.transform.position.x,yPos, PitchSetup.transform.position.z);
       //RollSetup.transform.position = new Vector3(RollSetup.transform.position.x,yPos, RollSetup.transform.position.z);
        //OrientationCross.transform.position =  new Vector3(OrientationCross.transform.position.x,yPos, OrientationCross.transform.position.z);
        FixationPoint.transform.position = new Vector3(OrientationCross.transform.position.x,OrientationCross.transform.position.y, OrientationCross.transform.position.z);
    }
    
    private void SetCalibrationStatus()
    {
        OrientationCross.SetActive(true);
        FixationCrossObject.SetActive(true);
        FixationPoint.SetActive(false);
        foreach (Transform child in OrientationCross.transform)
        {
            child.transform.gameObject.SetActive(true);
        }

        ExperimentStatus = false;

        CalibrationStatus = true;

        _isReadyToGo = false;


        //FixationCross.SetActive(true);
    }
    
    
    IEnumerator MakeReady(float sec)
    {
        if (!isInResetStatus)
        {
            isInResetStatus = true;
            yield return new WaitForSeconds(sec);
            _isReadyToGo = true;
            isInResetStatus = false;
        }

        
    }
    
    
    
    IEnumerator PeriodicallyResetCameraPosition(float sec)
    {
        isPeriodicallyResetting=true;
        
        while (!fixationCross.GetAlignment())
        {
            Debug.Log("periodic allignment");
            yield return new WaitForSeconds(sec);
            ResetCameraPosition();
        }

        isPeriodicallyResetting = false;

    }
    

    private void SetExperimentStatus()
    {
        _isReadyToGo = true;
        OrientationCross.SetActive(false);

        FixationPoint.SetActive(true);

        foreach (Transform child in OrientationCross.transform)
        {
            child.transform.gameObject.SetActive(false);
        }

        Counter = CountdownUntilAligned;

        ExperimentStatus = true;

        CalibrationStatus = false;
    }



    private void SetYawPositionActive(int i)
    {
        YawSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }

    private void SetPitchPositionActive(int i)
    {
        PitchSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }
    
    private void SetRollPositionActive(int i)
    {
        RollSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
    }


    public void RunYaw(HeadMovement yawMovement)
    {
        _yawMovement = yawMovement;
        _isYaw = true;
        _isRoll = false;
        _isPitch = false;

        CalibrationStatus = true;


        // todo start yaw
    }
    
    public void RunPitch(HeadMovement pitchMovement)
    {
        _pitchMovement = pitchMovement;
        _isYaw = false;
        _isRoll = false;
        _isPitch = true;
        
        CalibrationStatus = true;

        // todo start pitch
    }
    
    public void RunRoll(HeadMovement rollMovement)
    {
        _rollMovement = rollMovement;
        _isYaw = false;
        _isRoll = true;
        _isPitch = false;
        
        CalibrationStatus = true;

        // todo start roll
    }
    
}
