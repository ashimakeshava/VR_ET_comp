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

    private FixationCross fixationCross;
    // Start is called before the first frame update
    void Start()
    {
        fixationCross = FixationCrossObject.GetComponent<FixationCross>();
        
    }

    // Update is called once per frame
    void Update()
    {
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
            if (PitchSetup.activeInHierarchy)
            {
                PitchSetup.SetActive(false);
            }
            else
            {
                PitchSetup.SetActive(true);
            }
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
          
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("plop");

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



       
    }
    
    private void SetCalibrationStatus()
    {
        OrientationCross.transform.GetChild(0).gameObject.SetActive(true);
        OrientationCross.transform.GetChild(1).gameObject.SetActive(true);
        //FixationCross.SetActive(true);
    }

    private void SetExperimentStatus()
    {
        //FixationCross.SetActive(false);
        OrientationCross.transform.GetChild(0).gameObject.SetActive(false);
        OrientationCross.transform.GetChild(1).gameObject.SetActive(false);
    }
}
