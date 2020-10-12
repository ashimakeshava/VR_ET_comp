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
            if (RollSetup.activeInHierarchy)
            {
                RollSetup.SetActive(false);
            }
            else
            {
                RollSetup.SetActive(true);
            }
        }
          
        
    }

    private void ResetCameraPosition()
    {
        //Todd Wassen miracle solution for alligning the WorldSpace to the Camera
        Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        Valve.VR.OpenVR.Compositor.SetTrackingSpace(
            Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);

        float yPos = Camera.main.transform.position.y;

        YawSetup.transform.position = new Vector3(YawSetup.transform.position.x,yPos, YawSetup.transform.position.z);
        PitchSetup.transform.position = new Vector3(PitchSetup.transform.position.x,yPos, PitchSetup.transform.position.z);
        RollSetup.transform.position = new Vector3(RollSetup.transform.position.x,yPos, RollSetup.transform.position.z);
        OrientationCross.transform.position =  new Vector3(OrientationCross.transform.position.x,yPos, OrientationCross.transform.position.z);
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



    private void SetYawPosition(int i)
    {
        YawSetup.transform.GetChild(i).transform.gameObject.SetActive(true);
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
