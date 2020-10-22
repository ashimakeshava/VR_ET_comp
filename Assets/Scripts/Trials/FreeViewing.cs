using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeViewing : MonoBehaviour
{
    [SerializeField] private List<GameObject> _pictures;
    
    private GameObject _fixationPoint;
    private List<FreeViewingDataFrame> _freeViewingDataFrames;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _freeViewingDataFrames = new List<FreeViewingDataFrame>();
    }

    IEnumerator StartFreeViewing()
    {
        foreach (var frame in _freeViewingDataFrames)
        {
            _fixationPoint.transform.localPosition = Vector3.zero;
            _fixationPoint.gameObject.SetActive(true);
            ExperimentManager.Instance.SetFixationPointActivationStatus(true);
            ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

            yield return new WaitForSeconds(frame.FixationPointDuration);
            
            _fixationPoint.gameObject.SetActive(false);
            ExperimentManager.Instance.SetFixationPointActivationStatus(false);

            _pictures[frame.IndexofTheObject].SetActive(true);
            ExperimentManager.Instance.SetStimuliActivationStatus(true);

            yield return new WaitForSeconds(frame.PhotoFixationDuration);
            
            _pictures[frame.IndexofTheObject].SetActive(false);
            ExperimentManager.Instance.SetStimuliActivationStatus(false);
        }
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunFreeViewing(List<FreeViewingDataFrame> dataFrames)
    {
        _freeViewingDataFrames = dataFrames;
        StartCoroutine(StartFreeViewing());
    }
}
