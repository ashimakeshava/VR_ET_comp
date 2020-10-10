using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroSaccades : MonoBehaviour
{
    private GameObject _fixationPoint;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }

    IEnumerator StartMicroSaccades()
    {
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(30);
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunMicroSaccades()
    {
        StartCoroutine(StartMicroSaccades());
    }
}
