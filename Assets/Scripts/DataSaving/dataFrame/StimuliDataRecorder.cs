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
    }

    public void StartStimuliDataRecording()
    {
        StartCoroutine(RecordStimuliEvents());
        _runningRecording = true;
    }

    public void StopStimuliDataRecording(string number)
    {
        string fileName = "BlockStimuliData" + number;
        _runningRecording = false;
        
        DataSavingManager.Instance.SaveList(_stimuliDataFrames, fileName);
        _stimuliDataFrames.Clear();
    }

    private IEnumerator RecordStimuliEvents()
    {
        while (_runningRecording)
        {
            StimuliDataFrame data = new StimuliDataFrame
            {
                UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp(),
                FixationPointActive = ExperimentManager.Instance.GetFixationPointActivationStatus(),
                FixationPointPosition = ExperimentManager.Instance.GetFixationPointPosition(),
                StimuliActive = ExperimentManager.Instance.GetStimuliActivationStatus(),
                HeadMovementStimuliActive = ExperimentManager.Instance.GetHeadMovementStimuliActivationStatus(),
                HeadMovementObjectName = ExperimentManager.Instance.GetHeadMovementObjectName(),
                SpacePressed = ExperimentManager.Instance.GetSpacePressedStatus(),
                TrialActive = ExperimentManager.Instance.GetTrialActivationStatus()
            };
            
            _stimuliDataFrames.Add(data);
            
            yield return new WaitForSeconds(_sampleRate);
        }
    }
}
