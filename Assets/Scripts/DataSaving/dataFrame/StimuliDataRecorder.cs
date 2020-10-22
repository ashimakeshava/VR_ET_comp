using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StimuliDataRecorder : MonoBehaviour
{
    private List<StimuliDataFrame> _stimuliDataFrames;
    private bool _runningRecording;
    private readonly float _sampleRate = 1f / 90f;


    private void Awake()
    {
        _stimuliDataFrames = new List<StimuliDataFrame>();
        _runningRecording = false;
    }

    public void StartStimuliDataRecording()
    {
        _runningRecording = true;
        StartCoroutine(RecordStimuliEvents());
    }

    public void StopStimuliDataRecording(string id, string number)
    {
        string fileName = id + "_Stimuli_Data_Varjo_Block_" + number;
        _runningRecording = false;
        
        DataSavingManager.Instance.SaveList(_stimuliDataFrames, fileName);
        _stimuliDataFrames.Clear();
    }

    private IEnumerator RecordStimuliEvents()
    {
        while (_runningRecording)
        {
            StimuliDataFrame data = new StimuliDataFrame();

            data.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();
            data.FPS = GetComponent<FPSDisplay>().GetCurrentFPS();
            data.TrialsID = ExperimentManager.Instance.GetTrialsID();
            data.FixationPointActive = ExperimentManager.Instance.GetFixationPointActivationStatus();
            data.FixationPointPosition = ExperimentManager.Instance.GetFixationPointPosition();
            data.GlobalFixationPointActive = ExperimentManager.Instance.GetGlobalFixationPointActivationStatus();
            data.GlobalFixationPointPosition = ExperimentManager.Instance.GetGlobalFixationPointPosition();
            data.StimuliActive = ExperimentManager.Instance.GetStimuliActivationStatus();
            data.HeadMovementStimuliActive = ExperimentManager.Instance.GetHeadMovementStimuliActivationStatus();
            data.HeadMovementObjectName = ExperimentManager.Instance.GetHeadMovementObjectName();
            data.SpacePressed = ExperimentManager.Instance.GetSpacePressedStatus();
            data.TrialActive = ExperimentManager.Instance.GetTrialActivationStatus();
            data.ContrastVariationName = ExperimentManager.Instance.GetContrastVariationName();
            
            _stimuliDataFrames.Add(data);

            yield return new WaitForSeconds(_sampleRate);
        }
    }
}
