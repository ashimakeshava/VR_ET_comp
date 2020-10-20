using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StimuliDataRecorder : MonoBehaviour
{
    private bool runningRecording;
    
    private void StartRecording()
    {
        StartCoroutine(RecordStimuliEvents);
        runningRecording = true;
    }

    public IEnumerator RecordStimuliEvents()
    {
        while (runningRecording)
        {
            StimuliDataFrame data = new StimuliDataFrame();

            data.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();

            data.StimuliOnset = ExperimentManager.Instance.GetStimuliOnset()
        }
    }
    
    
    
    public 
    
}
