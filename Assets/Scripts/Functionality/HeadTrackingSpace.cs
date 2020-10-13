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

    private FixationCross fixationCross;

    [SerializeField] private GameObject FixationPoint;

    private HeadMovement _yawMovement, _pitchMovement, _rollMovement;
    

    [SerializeField] private float CountdownAligned=5;
    // Start is called before the first frame update
    void Start()
    {
        CalibrationStatus = true;
        fixationCross = FixationCrossObject.GetComponent<FixationCross>();
        _yawMovement = _pitchMovement = _rollMovement = new HeadMovement();
    }

    // Update is called once per frame
    void Update()
    {
        // if (CalibrationStatus)
        // {
        //     ResetCameraCouroutineInCalibrationStatus(2f);
        // }
        if (CalibrationStatus)
        {
            if (fixationCross.GetAlignment())
            {
                CountdownAligned -= Time.deltaTime;
                Debug.Log(CountdownAligned);
            }
            else
            {
                CountdownAligned = 3f;
                SetCalibrationStatus();
            }
            if (CountdownAligned <= 0f)
            {
                SetExperimentStatus();
            }
        }
        
        

        
        //if(receivedEyetrackingGaze)
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            fixationCross.DisableVertical();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (YawSetup.activeInHierarchy)
            {
                YawSetup.SetActive(false);
            }
            else
            {
                YawSetup.SetActive(true);
            }
        }
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            ResetCameraPosition();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            fixationCross.DisableVertical();
            PitchSetup.SetActive(false);
            RollSetup.SetActive(true);
            YawSetup.SetActive(false);
            foreach (Transform pos in RollSetup.transform)
            {
                pos.gameObject.SetActive(false);
            }

            int random = Random.Range(0, 5);
            SetRollPositionActive(random);
            
            fixationCross.SetTargetObject(RollSetup.transform.GetChild(random).gameObject);
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            fixationCross.DisableVertical();
            PitchSetup.SetActive(true);
            RollSetup.SetActive(false);
            YawSetup.SetActive(false);
            foreach (Transform pos in PitchSetup.transform)
            {
                pos.gameObject.SetActive(false);
            }

            int random = Random.Range(0, 5);

            SetPitchPositionActive(random);
                
            
            fixationCross.SetTargetObject(PitchSetup.transform.GetChild(random).gameObject);
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            fixationCross.DisableHorizontal();
            PitchSetup.SetActive(false);
            RollSetup.SetActive(false);
            YawSetup.SetActive(true);
            foreach (Transform pos in YawSetup.transform)
            {
                pos.gameObject.SetActive(false);
            }

            int random = Random.Range(0, 5);

            SetYawPositionActive(random);
                
            
            fixationCross.SetTargetObject(YawSetup.transform.GetChild(random).gameObject);
        }
        
          
        
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
        FixationPoint.transform.position = new Vector3(OrientationCross.transform.position.x,yPos, OrientationCross.transform.position.z);
    }
    
    private void SetCalibrationStatus()
    {
        foreach (Transform child in OrientationCross.transform)
        {
            child.transform.gameObject.SetActive(true);
        }

        ExperimentStatus = true;

        CalibrationStatus = true;


        //FixationCross.SetActive(true);
    }

    private void ResetCameraCouroutineInCalibrationStatus(float seconds)
    {
    }
    IEnumerator PeriodicallyResetCameraPosition(float sec)
    {
        ResetCameraPosition();
        yield return new WaitForSeconds(sec);
    }
    

    private void SetExperimentStatus()
    {
        //FixationCross.SetActive(false);

        foreach (Transform child in OrientationCross.transform)
        {
            child.transform.gameObject.SetActive(false);
        }

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
        // todo start yaw
    }
    
    public void RunPitch(HeadMovement pitchMovement)
    {
        _pitchMovement = pitchMovement;
        // todo start pitch
    }
    
    public void RunRoll(HeadMovement rollMovement)
    {
        _rollMovement = rollMovement;
        // todo start roll
    }
    
}
