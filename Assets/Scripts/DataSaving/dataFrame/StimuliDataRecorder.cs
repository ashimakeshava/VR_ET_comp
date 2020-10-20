using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StimuliDataRecorder : MonoBehaviour
{
    private List<StimuliDataFrame> _stimuliDataFrames;
    private bool _runningRecording;


    private void Start()
    {
        _stimuliDataFrames = new List<StimuliDataFrame>();
    }

    public void StartStimuliDataRecording()
    {
        _stimuliDataFrames.Clear();
        StartCoroutine(RecordStimuliEvents());
        _runningRecording = true;
    }

    public void StopStimuliDataRecording(string number)
    {
        string fileName = "BlockStimuliData" + number;
        _runningRecording = false;
        DataSavingManager.Instance.SaveList(_stimuliDataFrames, fileName);
    }

    private IEnumerator RecordStimuliEvents()
    {
        while (_runningRecording)
        {
            StimuliDataFrame data = new StimuliDataFrame();

            data.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();

            data.FixationPointActive = ExperimentManager.Instance.GetFixationPointActivationStatus();
            data.FixationPointPosition = ExperimentManager.Instance.GetFixationPointPosition();
            
            data.StimuliActive = ExperimentManager.Instance.GetStimuliActivationStatus();

            data.HeadMovementStimuliActive = ExperimentManager.Instance.GetHeadMovementStimuliActivationStatus();
            data.HeadMovementObjectName = ExperimentManager.Instance.GetHeadMovementObjectName();

            data.SpacePressed = ExperimentManager.Instance.GetSpacePressedStatus();

            data.TrialActive = ExperimentManager.Instance.GetTrialActivationStatus();

            _stimuliDataFrames.Add(data);
            yield return null;
        }
    }
}
