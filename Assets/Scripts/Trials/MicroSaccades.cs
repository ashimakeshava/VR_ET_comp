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
        _grid = ExperimentManager.Instance.GetGrid();    // todo change to only collider since at the runtime we won't have large and small grid
    }

    IEnumerator StartMicroSaccades()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(30);
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunMicroSaccades()
    {
        StartCoroutine(StartMicroSaccades());
    }
}
