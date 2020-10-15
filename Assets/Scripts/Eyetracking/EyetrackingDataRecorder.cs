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
            
            var eyeTrackingDataWorld = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            var eyeTrackingDataLocal = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
            
            
            //SRanipal_GazeRaySample_v2 sample = SRanipal_Eye_v2.GetGazeRay()
            if (eyeTrackingDataWorld.GazeRay.IsValid)
            {
//                Debug.Log("valid eyetracking data");
                Vector3 gazeRayDirection = eyeTrackingDataWorld.GazeRay.Direction;
               // dataFrame.TobiiTimeStamp = eyeTrackingDataWorld.Timestamp;

                Ray rayLeft;

                Ray rayRight;

                Ray rayCombined;

                SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out rayLeft);

                SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out rayRight);
                
                SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out rayCombined);
                    
                //bool isOpen = SRanipal_Eye_v2.GetEyeOpenness()
                Ray ray;
                
                
                
                // if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out ray))
                // {
                //     
                //     Vector3 origin= eyeTrackingDataLocal.GazeRay.Origin;
                //     Vector3 direction =eyeTrackingDataLocal.GazeRay.Direction;
                //     //Vector3 origin = ray.origin;
                //     //Vector3 direction = ray.direction;
                //     directionLeft.transform.localRotation= Quaternion.Euler(gazeRayDirection);
                //
                //     RaycastHit hit;
                //     if (Physics.Raycast(directionLeft.transform.position, transform.TransformDirection(direction),
                //         out hit))
                //     {
                //         Debug.Log(hit.collider.gameObject.name + directionLeft.name);
                //     }
                //
                //
                // }
                
                if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out ray))
                {
                    //Vector3 origin= eyeTrackingDataWorld.GazeRay.Direction;
                    //Vector3 direction =eyeTrackingDataWorld.GazeRay.Origin;
                   // directionRight.transform.rotation= Quaternion.FromToRotation(origin,direction);
                    RaycastHit hit;
                   // if (Physics.Raycast(directionRight.transform.position, transform.TransformDirection(direction),
                 //       out hit))
                   // {
                   //     Debug.Log(hit.collider.gameObject.name + directionRight.name);
                   //}

                   
                }
                
                if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out ray))
                {
                        Vector3 origin = ray.origin;
                        Vector3 direction = ray.direction;
                       
                    
                    RaycastHit hit;
                    if (Physics.Raycast(origin, direction,
                        out hit))
                    {
                        Debug.Log(hit.collider.gameObject.name + directionCombined.name);
                        
                        
                        directionCombined.transform.localRotation= Quaternion.FromToRotation(directionCombined.transform.forward, hit.point);
                    }
                    
                    
                    
                }

                
                
                
                Vector3 posLeftEye = rayLeft.origin;
                Vector3 dirLeftEye = rayLeft.direction;
                
                Vector3 posRightEye = rayRight.origin;
                Vector3 dirRightEye = rayRight.direction;
                
                Vector3 posCombinedEyes =rayCombined.origin;
                Vector3 dirCombinedEyes = rayCombined.direction;
                
                //Debug.Log(posLeftEye+ dirLeftEye);
                
                //Debug.Log(posRightEye+ dirRightEye);
                
                
               // Debug.DrawRay(rayLeft.origin, rayLeft.direction, Color.red,5f);
                
                //Debug.DrawLine(rayLeft.origin, rayLeft.direction, Color.green,5f);
                
               // Debug.DrawRay(rayRight.origin, rayRight.direction,Color.blue,5f);
            }

            /*if (eyeTrackingDataLocal.GazeRay.IsValid)
            {
                dataFrame.EyePosLocalCombined = eyeTrackingDataLocal.GazeRay.Origin;
                dataFrame.EyeDirLocalCombined = eyeTrackingDataLocal.GazeRay.Direction;
                dataFrame.LeftEyeIsBlinkingLocal = eyeTrackingDataLocal.IsLeftEyeBlinking;
                dataFrame.RightEyeIsBlinkingLocal = eyeTrackingDataLocal.IsRightEyeBlinking;
            }

            dataFrame.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();
            dataFrame.FPS = SavingManager.Instance.GetCurrentFPS();
            
            dataFrame.HmdPosition = EyetrackingManager.Instance.GetHmdTransform().position;

            dataFrame.NoseVector =  EyetrackingManager.Instance.GetHmdTransform().forward;
            
            _frameRates.Add(dataFrame.FPS);
            _recordedEyeTrackingData.Add(dataFrame);
            frameCounter++;*/
            
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
