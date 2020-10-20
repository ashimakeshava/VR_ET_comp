using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tobii.XR;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViveSR.anipal.Eye;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Varjo;

public class EyetrackingDataRecorder : MonoBehaviour
{
    // Start is called before the first frame update
    private float _sampleRate;
    private List<EyeTrackingDataFrame> _recordedEyeTrackingData;
    private List<float> _frameRates;
    private EyetrackingManager _eyetrackingManager;
    private Transform _hmdTransform;
    private bool recordingEnded;

    public GameObject directionLeft;
    public GameObject directionRight;
    public GameObject directionCombined;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _recordedEyeTrackingData= new List<EyeTrackingDataFrame>();
        
        _eyetrackingManager= EyetrackingManager.Instance;
        
        _sampleRate = _eyetrackingManager.GetSampleRate();
        _hmdTransform = _eyetrackingManager.GetHmdTransform();
        
        
        if (!VarjoPlugin.InitGaze()) {
            Debug.LogError("Failed to initialize gaze");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.P))
        {
            Visualisation();
            Debug.Log("<color=green>Visualisation activated!</color>");
        }*/
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)  // generally I am not proud of this call, but seems necessary for the moment.
    {
        _hmdTransform = _eyetrackingManager.GetHmdTransform();        //refresh the HMD transform after sceneload;
        //Debug.Log("hello new World");
    }

    
    public void StartRecording()
    {
        recordingEnded = false;
        StartCoroutine(RecordEyeTrackingData());
    }

    public void StopRecording()
    {
        recordingEnded = true;
    }

    public void ClearEyeTrackingDataRecordings()
    {
        _recordedEyeTrackingData.Clear();
    }
    
    private IEnumerator RecordEyeTrackingData()
    {
        int frameCounter = new int();
        Debug.Log("<color=green>Start recording...</color>");

        
        _frameRates = new List<float>();
        
        while (!recordingEnded)
        {
            var dataFrame = new VR_ET_com_EyetrackingDataFrame();
            
            //var eyeTrackingDataWorld = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            //var eyeTrackingDataLocal = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
            var gazedata= VarjoPlugin.GetGaze();

            if (gazedata.status != VarjoPlugin.GazeStatus.INVALID)
            {
                dataFrame.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();
                Debug.Log("position length" +gazedata.left.position.Length);
                Debug.Log("foward length" +gazedata.left.forward.Length);
                
                dataFrame.eyePositionLeftWorld = new Vector3((float) gazedata.left.position[0],(float) gazedata.left.position[1], (float) gazedata.left.position[2]);  //might be also local and not world space need to check that
                dataFrame.eyeDirectionLeftWorld = new Vector3((float) gazedata.left.forward[0],(float) gazedata.left.forward[1], (float) gazedata.left.forward[2]);
                
                dataFrame.eyePositionRightWorld = new Vector3((float) gazedata.right.position[0],(float) gazedata.right.position[1], (float) gazedata.right.position[2]);
                dataFrame.eyeDirectionRightWorld=  new Vector3((float) gazedata.right.forward[0],(float) gazedata.right.forward[1], (float) gazedata.right.forward[2]);
                
                dataFrame. eyePositionWorldCombined = new Vector3((float) gazedata.gaze.position[0],(float) gazedata.gaze.position[1], (float) gazedata.gaze.position[2]);
                dataFrame.eyeDirectionWorldCombined = new Vector3((float) gazedata.gaze.position[0],(float) gazedata.gaze.position[1], (float) gazedata.gaze.position[2]);
                
                /*dataFrame.eyeDirectionLeftLocal.x = (float) gazedata.left.forward[0];//currently not happy about it;
                dataFrame.eyeDirectionLeftLocal.y = (float) gazedata.left.forward[1];
                dataFrame.eyeDirectionLeftLocal.z = (float) gazedata.left.forward[2];*/
            }
            
            yield return new WaitForSeconds(_sampleRate);
        }
        
            
    }


    private List<HitObjectInfo> GetHitObjectsFromGaze(Vector3 gazeOrigin, Vector3 gazeDirection)
    {
        RaycastHit[] hitColliders = Physics.RaycastAll(gazeOrigin, gazeDirection);
        
        List<HitObjectInfo> hitObjectInfoList= new List<HitObjectInfo>();
        
        foreach (var colliderhit in hitColliders)
        {
                    
            HitObjectInfo hitInfo = new HitObjectInfo();
            hitInfo.ObjectName = colliderhit.collider.gameObject.name;
            hitInfo.HitObjectPosition = colliderhit.collider.transform.position;
            hitInfo.HitPointOnObject = colliderhit.point;
            hitObjectInfoList.Add(hitInfo);
        }

        return hitObjectInfoList;
    }

    private List<HitObjectInfo> GetFirstHitObjectFromGaze(Vector3 gazeOrigin, Vector3 gazeDirection, float distance)
    {
        RaycastHit hit;
        bool hitColliders = Physics.Raycast(gazeOrigin, gazeDirection, out hit, distance);
        
        List<HitObjectInfo> hitObjectInfoList= new List<HitObjectInfo>();

        if (hitColliders)
        {
            HitObjectInfo hitInfo = new HitObjectInfo();
            hitInfo.ObjectName = hit.collider.gameObject.name;
            hitInfo.HitObjectPosition = hit.collider.transform.position;
            hitInfo.HitPointOnObject = hit.point;
            hitObjectInfoList.Add(hitInfo);
        }

        return hitObjectInfoList;
    }


    public List<EyeTrackingDataFrame> GetDataFrames()
    {
        if (recordingEnded)
        {
            return _recordedEyeTrackingData;
        }
        else
        {
            throw new Exception("Eyetracking Data Recording has not been finished");
        }
    }

    public float GetAverageFrameRate()
    {
        return _frameRates.Average();
    }

    private void Visualisation()
    {
        List<EyeTrackingDataFrame> dataFrames = GetDataFrames();

        foreach (var dataFrame in dataFrames)
        {
            if (dataFrame.hitObjects != null)
            {
                foreach (var item in dataFrame.hitObjects)
                {
                    Debug.Log(item.ObjectName);
                    Debug.DrawLine(dataFrame.HmdPosition, item.HitPointOnObject, Color.red, 60f);
                }
            }
        }
    }
}
