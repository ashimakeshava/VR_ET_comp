using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StimuliDataRecorder : MonoBehaviour
{
    private bool runningRecording;
    
    public void StartRecording()
    {
        StartCoroutine(RecordStimuliEvents());
        runningRecording = true;
    }

    private IEnumerator RecordStimuliEvents()
    {
        while (runningRecording)
        {
            StimuliDataFrame data = new StimuliDataFrame();

            data.UnixTimeStamp = TimeManager.Instance.GetCurrentUnixTimeStamp();

            data.FixationPointOnSet = ExperimentManager.Instance.GetFixationPointOnset();
            data.FixationPointOffSet = ExperimentManager.Instance.GetFixationPointOffset();
            
            data.StimuliOnset = ExperimentManager.Instance.GetStimuliOnset();
            data.StimuliOffset = ExperimentManager.Instance.GetStimuliOffset();

            data.HeadMovementStimuliOnSet = ExperimentManager.Instance.GetHeadMovementStimuliOnSet();
            data.HeadMovementStimuliOffSet = ExperimentManager.Instance.GetHeadMovementStimuliOffSet();
            data.HeadMovementObjectName = ExperimentManager.Instance.GetHeadMovementObjectName();

            data.SpacePressed = ExperimentManager.Instance.GetSpacePressedStatus();

            data.TrialStarted = ExperimentManager.Instance.GetTrialEndStatus();
            data.TrialEnded = ExperimentManager.Instance.GetTrialEndStatus();

            yield return null;
        }
    }
}
