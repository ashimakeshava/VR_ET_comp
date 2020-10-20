using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingManager : MonoBehaviour
{
    public string DataName;
    private string _GUIDFolderPath;
    public static SavingManager Instance { get; private set; }
    public int SetSampleRate = 90;
    private float _sampleRate;

    private List<EyeTrackingDataFrame> _eyeTrackingData;

    // TODO check if necessary at all
    // private CalibrationData _participantCalibrationData;

    private bool _readyToSaveToFile;

    private List<string[]> rawData;
    private List<float> _frameRates;

    private GameObject participantCar;
    private string _targetSceneName;
    private string _desktopPath;
    private string _desktopFolderPath;
    
    private void Awake()
    {
        _sampleRate = 1f / SetSampleRate;
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        
        _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _desktopFolderPath = Path.GetFullPath(Path.Combine(_desktopPath, "WestdriveLoopARData"));
    }

    void Start()
    {
        _frameRates = new List<float>();
        _readyToSaveToFile=false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift))
        {
            TimeManager.Instance.SetExperimentEndTime();
            StopAndSaveData("Incomplete");
        }
    }

    public float GetSampleRate()
    {
        return _sampleRate;
    }
    
    public void StartRecordingData()
    {
        RecordData();
    }

    public void StopRecordingData()
    {
        StopRecord();
    }

    public void SaveData()
    {
        _readyToSaveToFile = TestCompleteness();
        
        if (_readyToSaveToFile)
        {
            SaveToJson();
        }
        else
        {
            Debug.Log("error the data collection was not completed or corrupted");
        }
    }

    IEnumerator SavingData()
    {
        _readyToSaveToFile = TestCompleteness();
        
        if (_readyToSaveToFile)
        {
            // TODO activate after adjustments
            // yield return SavingToJson();
        }
        else
        {
            Debug.Log("error the data collection was not completed or corrupted");
            yield return null;
        }
    }

    IEnumerator StartRecordingAfterSavingData()
    {
        StopRecord();
        Debug.Log("<color=red>Stopped recording Data...</color>");
        
        yield return SavingData();
        Debug.Log("<color=blue>Saving Data...</color>");

        _eyeTrackingData.Clear();

        StartRecordingData();
    }

    public void StopAndSaveData(string targetScene=null)
    {
        _targetSceneName = targetScene;
        StartCoroutine(StopingAndSavingData());
    }
    
    IEnumerator StopingAndSavingData()
    {
        StopRecord();
        Debug.Log("<color=red>Stopped recording Data...</color>");
        
        yield return SavingData();
        Debug.Log("<color=blue>Saving Data...</color>");
        
        _eyeTrackingData.Clear();
    }
    
    private void RecordData()
    {
        _readyToSaveToFile = false;
        Debug.Log("<color=green>Recording Data...</color>");
        EyetrackingManager.Instance.StartRecording();
    }

    private void StopRecord()
    {
        Debug.Log("<color=red>Stop recording Data!</color>");
        EyetrackingManager.Instance.StopRecording();
       // RetrieveData();
    }

//    private void RetrieveData()
//    {
//        StoreEyeTrackingData(EyetrackingManager.Instance.GetEyeTrackingData());
//
//        // TODO check necessity
//        // StoreCalibrationData();
//    }

    private bool TestCompleteness()
    {
        if (_eyeTrackingData != null /*&& _participantCalibrationData != null*/)
            return true;
        return false;
    }

    public void StoreEyeTrackingData(List<EyeTrackingDataFrame> eyeTrackingDataFrames)
    {
        _eyeTrackingData = eyeTrackingDataFrames;
    }

    /*public void StoreCalibrationData()
    {
        _participantCalibrationData = CalibrationManager.Instance.GetCalibrationData();
        _participantCalibrationData.AverageExperimentFPS = _frameRates.Average();
        _participantCalibrationData.ApplicationDuration = TimeManager.Instance.GetApplicationDuration();
        _participantCalibrationData.ExperimentDuration = TimeManager.Instance.GetExperimentDuration();
        _participantCalibrationData.TrainingSuccessState = CalibrationManager.Instance.GetTestDriveState();
        _participantCalibrationData.NumberOfTrainingTrials = CalibrationManager.Instance.GetTestDriveNumberOfTrials();
    }*/

    public void SetParticipantCar(GameObject car)
    {
        participantCar = car;
    }
    

    private List<String> ConvertToJson(List<EyeTrackingDataFrame> inputData)
    {
        List<string> list = new List<string>();
        list.Add("[");
        foreach(var frame in inputData)
        {
            string jsonString = JsonUtility.ToJson(frame, true);
            if(frame != inputData.Last())
                list.Add(jsonString + ",");
            else
                list.Add(jsonString);
        }        
        list.Add("]");
        return list;
    }
    
    
    public void SaveToJson()
    {
        if (_readyToSaveToFile)
        {
            var eyeTracking = ConvertToJson(_eyeTrackingData);
            
            // TODO check necessity
            // var participantCalibrationData = JsonUtility.ToJson(_participantCalibrationData);

            // TODO replace the id system
            /*var id = _participantCalibrationData.ParticipantUuid;

            using (FileStream stream = File.Open(GetPathForSaveFile(DataName, DataName, DataName), FileMode.Create))
            {
                File.WriteAllLines(GetPathForSaveFile("EyeTracking", id, _targetSceneName), eyeTracking);
            }*/


            // TODO check necessity
            /*using (FileStream stream = File.Open(GetPathForSaveParticipantCalibrationData(DataName, DataName), FileMode.Create))
            {
                File.WriteAllText(GetPathForSaveParticipantCalibrationData("ParticipantCalibrationData", id), participantCalibrationData);
            }*/
        }
        
        Debug.Log("saved to " + _desktopPath);
    }
    
    
    //TODO check above
    /*IEnumerator SavingToJson()
    {
        if (_readyToSaveToFile)
        {
            var input = ConvertToJson(_inputData);
            var eyeTracking = ConvertToJson(_eyeTrackingData);
            var sceneData = JsonUtility.ToJson(_sceneData);
            var participantCalibrationData = JsonUtility.ToJson(_participantCalibrationData);

            var id = _participantCalibrationData.ParticipantUuid;

            using (FileStream stream = File.Open(GetPathForSaveFile(DataName, DataName, DataName), FileMode.Create))
            {
                File.WriteAllLines(GetPathForSaveFile("Input", id, _targetSceneName), input);
            }
            
            using (FileStream stream = File.Open(GetPathForSaveFile(DataName, DataName, DataName), FileMode.Create))
            {
                File.WriteAllLines(GetPathForSaveFile("EyeTracking", id, _targetSceneName), eyeTracking);
            }
            
            using (FileStream stream = File.Open(GetPathForSaveFile(DataName, DataName, DataName), FileMode.Create))
            {
                File.WriteAllText(GetPathForSaveFile("SceneData", id, _targetSceneName), sceneData);
            }
            
            using (FileStream stream = File.Open(GetPathForSaveParticipantCalibrationData(DataName, DataName), FileMode.Create))
            {
                File.WriteAllText(GetPathForSaveParticipantCalibrationData("ParticipantCalibrationData", id), participantCalibrationData);
            }
        }
        
        Debug.Log("saved to " + _desktopPath);

        yield return null;
    }*/
    

    private string GetPathForSaveFile(string folderFileName, string id, string sceneName)
    {
        return Path.Combine(Path.GetFullPath(Path.Combine(_desktopFolderPath, folderFileName)), id + "_" + folderFileName + "_" + sceneName + ".txt");
    }
    
    private string GetPathForSaveParticipantCalibrationData(string folderFileName, string id)
    {
        return Path.Combine(Path.GetFullPath(Path.Combine(_desktopFolderPath, folderFileName)), id + "_" + folderFileName + ".txt");
    }

    public void SaveDataAndStartRecordingAgain(string oldSceneName)
    {
        _targetSceneName = oldSceneName;
        StartCoroutine(StartRecordingAfterSavingData());
    }

    public float GetCurrentFPS()
    {
        return this.gameObject.GetComponent<FPSDisplay>().GetCurrentFPS();
    }
}
