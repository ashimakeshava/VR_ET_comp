using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.XR;
using UnityEngine;
using UnityEngine.SceneManagement;
using Varjo;
using ViveSR.anipal.Eye;

public class EyetrackingManager : MonoBehaviour
{
    public static EyetrackingManager Instance { get; private set; }

    public int SetSampleRate = 90;
    private Transform _hmdTransform;
    private List<VR_ET_com_EyetrackingDataFrame> _eyeTrackingDataFrames;
    private EyeValidationData _eyeValidationData;
    private EyetrackingValidation _eyetrackingValidation;
    private bool _eyeValidationSucessful;
    private EyetrackingDataRecorder _eyeTrackingRecorder;
    private float _sampleRate;

    private bool _calibrationSuccess;

    private float eyeValidationDelay;

    private Vector3 _eyeValidationErrorAngles;
    
    public delegate void OnCompletedEyeValidation(bool wasSuccessful);
    public event OnCompletedEyeValidation NotifyEyeValidationCompletnessObservers;

    private bool _isCalibrated;
    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        //singleton pattern a la Unity
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);         //the Eyetracking Manager should be persitent by changing the scenes maybe change it on the the fly
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        //  I do not like this: we still needs tags to find that out.
    }

    private void Update()
    {
        if (VarjoPlugin.IsGazeCalibrated())
        {
            Debug.Log(VarjoPlugin.IsGazeCalibrated());
            _isCalibrated = true;
        }
        else
        {
            _isCalibrated = false;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
           StartRecording();
        }
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            StopRecording(null);
        }
        
        if(Input.GetKeyDown(KeyCode.K))
        {
            StartCalibration();
        }
        
        
        
        if(Input.GetKeyDown(KeyCode.O))
        {
            var quality = VarjoPlugin.GetGazeCalibrationQuality();
        
            Debug.Log("hello, the quality  is left " + (quality.left) + "and right "+  quality.right);
        }

        
    }
    
    

    private void  OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _hmdTransform = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        _sampleRate = 1f / SetSampleRate; 
        _eyeTrackingRecorder = GetComponent<EyetrackingDataRecorder>();
    }

    // Update is called once per frame
    



    public void StartCalibration()
    {
        VarjoPlugin.RequestGazeCalibration();
        
        var quality = VarjoPlugin.GetGazeCalibrationQuality();
        
    }

    public void StartRecording()
    {
        Debug.Log("<color=green>Recording eye-tracking Data!</color>");
        _eyeTrackingRecorder.StartRecording();
    }
    
    public void StopRecording(string blockNumber)
    {
        Debug.Log("<color=green> Stop Recording!</color>");
        _eyeTrackingRecorder.StopRecording();
        StoreEyeTrackingData();
        SaveEyetrackingData(_eyeTrackingDataFrames, blockNumber);
    }
    
    
    public Transform GetHmdTransform()
    {
        return _hmdTransform;
    }

    public float GetSampleRate()
    {
        return _sampleRate;
    }

    public void StoreEyeValidationData(EyeValidationData data)
    {
        _eyeValidationData = data;
    }

    public Vector3 GetEyeValidationErrorAngles()
    {
        return _eyeValidationErrorAngles;
    }
    

    private void StoreEyeTrackingData()
    {
        _eyeTrackingDataFrames = _eyeTrackingRecorder.GetDataFrames();
    }

    public List<VR_ET_com_EyetrackingDataFrame> GetEyeTrackingData()
    {
        if (_eyeTrackingDataFrames != null)
        {
            return _eyeTrackingDataFrames;
        }
        else
        {
            throw new Exception("There are no Eyetrackingdata");
        }
    }

    private void SetEyeValidationStatus(bool eyeValidationWasSucessfull, Vector3 errorAngles)
    {
        Debug.Log("eyeValidation Status was called in EyeTrackingManager with " + eyeValidationWasSucessfull);
        _eyeValidationSucessful = eyeValidationWasSucessfull;
        
        if (!eyeValidationWasSucessfull)
        {
            _eyeValidationErrorAngles = errorAngles;
            NotifyEyeValidationCompletnessObservers?.Invoke(false);
            
        }
        else
        {
            _eyeValidationErrorAngles = errorAngles;
            NotifyEyeValidationCompletnessObservers?.Invoke(true);
        }
    }

    public void SaveEyetrackingData(List<VR_ET_com_EyetrackingDataFrame> data, string blockNumber=null)
    {
        if (blockNumber == null)
        {
            blockNumber = TimeManager.Instance.GetCurrentUnixTimeStamp().ToString();
        }
        
        string fileName = "EyeTrackingBlock" + blockNumber;
        DataSavingManager.Instance.SaveList<VR_ET_com_EyetrackingDataFrame> (data, fileName);
    }
    
    public float GetAverageSceneFPS()
    {
        return _eyeTrackingRecorder.GetAverageFrameRate();
    }

    public bool GetEyeValidationStatus()
    {
        return _eyeValidationSucessful;
    }

    public bool IsCalibrated()
    {
        return _isCalibrated;
    }

    public double getCurrentTimestamp()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (System.DateTime.UtcNow - epochStart).TotalSeconds;
    }
}
