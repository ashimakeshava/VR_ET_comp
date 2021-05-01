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
    private List<VR_ET_com_EyetrackingDataFrame_vive> _recordedEyeTrackingData;
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
        _recordedEyeTrackingData= new List<VR_ET_com_EyetrackingDataFrame_vive>();
        
        _eyetrackingManager= EyetrackingManager.Instance;
        
        _sampleRate = _eyetrackingManager.GetSampleRate();
        _hmdTransform = _eyetrackingManager.GetHmdTransform();
        
        
        /*if (!VarjoPlugin.InitGaze()) {
            Debug.LogWarning("Varjo Failed to initialize gaze");
            gameObject.SetActive(false);
        }*/
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
            frameCounter++;
            //Debug.Log(frameCounter);
            var dataFrame = new VR_ET_com_EyetrackingDataFrame_vive();
            
            //var eyeTrackingDataWorld = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            //var eyeTrackingDataLocal = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
            //var gazedata= VarjoPlugin.GetGaze();
            SRanipal_Eye.GetVerboseData(out VerboseData verboseData);
            
            //SRanipal_Eye_v2.GetVerboseData(out VerboseData verboseData);

            var FixationPoint = ExperimentManager.Instance.GetFixationPoint();


            var leftEyeData= verboseData.left;
            var rightEyeData = verboseData.right;
            //validity check
                //left
            dataFrame.Left_DATA_GAZE_ORIGIN_VALIDITY = leftEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
            dataFrame.Left_DATA_EYE_OPENNESS_VALIDITY = leftEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY);
            dataFrame.Left_DATA_GAZE_DIRECTION_VALIDITY= leftEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
            dataFrame.Left_DATA_PUPIL_DIAMETER_VALIDITY = leftEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY);
            dataFrame.Left_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY = leftEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY);
            
                //right
            dataFrame.Right_DATA_GAZE_ORIGIN_VALIDITY = rightEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
            dataFrame.Right_DATA_EYE_OPENNESS_VALIDITY = rightEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY);
            dataFrame.Right_DATA_GAZE_DIRECTION_VALIDITY = rightEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
            dataFrame.Right_DATA_PUPIL_DIAMETER_VALIDITY = rightEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY);
            dataFrame.Right_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY = rightEyeData.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY);

            
            
            dataFrame.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();

            //HMD
            dataFrame.HmdPosition = _hmdTransform.transform.position;
            dataFrame.HmdRotation = _hmdTransform.rotation;
            dataFrame.NoseVector = _hmdTransform.transform.forward;

            Vector3 hmdPosition = _hmdTransform.transform.position;


            //Left Data
            Vector3 coordinateAdaptedGazeDirectionLeft = new Vector3(leftEyeData.gaze_direction_normalized.x * -1,  leftEyeData.gaze_direction_normalized.y, leftEyeData.gaze_direction_normalized.z);
                //local
            dataFrame.EyePositionLeftLocal= leftEyeData.gaze_origin_mm;
            dataFrame.EyeDirectionLeftLocal = coordinateAdaptedGazeDirectionLeft;
                //global
            dataFrame.EyePositionLeftWorld= leftEyeData.gaze_origin_mm / 1000 + _hmdTransform.position;
            dataFrame.EyeDirectionLeftWorld = _hmdTransform.rotation * coordinateAdaptedGazeDirectionLeft;
            
            

            dataFrame.EyeOpennessLeftSranipal = leftEyeData.eye_openness;
            dataFrame.pupilDiameterMillimetersLeft = leftEyeData.pupil_diameter_mm;
            
            
            //Right Data
            Vector3 coordinateAdaptedGazeDirectionRight = new Vector3(rightEyeData.gaze_direction_normalized.x * -1,  rightEyeData.gaze_direction_normalized.y, rightEyeData.gaze_direction_normalized.z);
            dataFrame.EyePositionRightLocal = rightEyeData.gaze_origin_mm;
            dataFrame.EyeDirectionRightLocal = coordinateAdaptedGazeDirectionRight;
            
            dataFrame.EyePositionRightWorld = rightEyeData.gaze_origin_mm / 1000 + _hmdTransform.position;
            dataFrame.EyeDirectionRightWorld = _hmdTransform.rotation * coordinateAdaptedGazeDirectionRight;


            dataFrame.EyeOpennessRightSranipal = rightEyeData.eye_openness;
            dataFrame.pupilDiameterMillimetersRight = rightEyeData.pupil_diameter_mm;
            
            //Combined Data
            var combinedData= verboseData.combined;
            
            Vector3 coordinateAdaptedGazeDirectionCombined = new Vector3(verboseData.combined.eye_data.gaze_direction_normalized.x * -1,  combinedData.eye_data.gaze_direction_normalized.y, combinedData.eye_data.gaze_direction_normalized.z);
            dataFrame.EyePositionCombinedLocal = combinedData.eye_data.gaze_origin_mm;
            dataFrame.EyeDirectionCombinedLocal = coordinateAdaptedGazeDirectionCombined;

            dataFrame.EyePositionCombinedWorld = combinedData.eye_data.gaze_origin_mm / 1000 + _hmdTransform.position;
            dataFrame.EyeDirectionCombinedWorld = _hmdTransform.rotation * coordinateAdaptedGazeDirectionCombined;
            
            
            
            

                var anglesLeft = Quaternion.FromToRotation((FixationPoint.transform.position - hmdPosition).normalized,
                    _hmdTransform.rotation * dataFrame.EyeDirectionLeftWorld).eulerAngles;

                dataFrame.ValidationErrorLeft = anglesLeft;
                
                
                var anglesRight = Quaternion.FromToRotation((FixationPoint.transform.position - hmdPosition).normalized,
                    _hmdTransform.rotation * dataFrame.EyeDirectionRightWorld).eulerAngles;
                
                dataFrame.ValidationErrorRight = anglesRight;
                
                
                
                     
                var anglesCombined = Quaternion.FromToRotation((FixationPoint.transform.position - hmdPosition).normalized,
                    _hmdTransform.rotation * dataFrame.EyeDirectionCombinedWorld).eulerAngles;
                
                dataFrame.ValidationErrorCombined = anglesCombined;
                
                
                
             
                
                HitObjectInfo hit= GetFirstHitObjectFromGaze(dataFrame.EyePositionCombinedWorld, dataFrame.EyeDirectionCombinedWorld);

                
               
                dataFrame.HitPositionOnTarget = hit.HitPointOnObject;
                dataFrame.PositionOfTarget = FixationPoint.transform.position;

                dataFrame.nameOfObject = hit.ObjectName;
                
                Debug.DrawRay(dataFrame.EyePositionCombinedWorld,dataFrame.EyeDirectionCombinedWorld, Color.red,3f);
                
                _recordedEyeTrackingData.Add(dataFrame);

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

    private HitObjectInfo GetFirstHitObjectFromGaze(Vector3 gazeOrigin, Vector3 gazeDirection, float distance = 100f)
    {
        RaycastHit hit;
        bool hitColliders = Physics.Raycast(gazeOrigin, gazeDirection, out hit, distance);
        
        HitObjectInfo hitInfo = new HitObjectInfo();
        if (hitColliders)
        {
            hitInfo.ObjectName = hit.collider.gameObject.name;
            hitInfo.HitObjectPosition = hit.collider.transform.position;
            hitInfo.HitPointOnObject = hit.point;
        }

        return hitInfo;
    }


    public List<VR_ET_com_EyetrackingDataFrame_vive> GetDataFrames()
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
        List<VR_ET_com_EyetrackingDataFrame_vive> dataFrames = GetDataFrames();

        foreach (var dataFrame in dataFrames)
        {
//            if (dataFrame.hitObjects != null)
//            {
//                foreach (var item in dataFrame.hitObjects)
//                {
//                    Debug.Log(item.ObjectName);
//                    Debug.DrawLine(dataFrame.HmdPosition, item.HitPointOnObject, Color.red, 60f);
//                }
//            }
        }
    }
}
