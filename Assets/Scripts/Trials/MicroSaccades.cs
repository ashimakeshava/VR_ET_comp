using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroSaccades : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _grid;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetGrid();
    }

    IEnumerator StartMicroSaccades()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        ExperimentManager.Instance.SetFixationPointActivationStatus(true);
        ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

        yield return new WaitForSeconds(30);
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunMicroSaccades()
    {
        StartCoroutine(StartMicroSaccades());
    }
}
