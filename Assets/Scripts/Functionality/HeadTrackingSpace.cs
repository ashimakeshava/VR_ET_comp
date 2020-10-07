using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HeadTrackingSpace : MonoBehaviour
{
    public GameObject OrientationCross;

    public GameObject YawSetup;

    public GameObject PitchSetup;

    public GameObject RollSetup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OrientationCross.transform.GetChild(0).gameObject.SetActive(false);
            OrientationCross.transform.GetChild(1).gameObject.SetActive(false);
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
}
