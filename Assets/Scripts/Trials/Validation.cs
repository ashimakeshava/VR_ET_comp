﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

public class Validation : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _gridClose;
    
    private List<GridElement> _gridElementsClose;
    private List<GridElement> _gridElementsFar;
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _gridClose = ExperimentManager.Instance.GetLargeGrid();
        _gridClose.gameObject.SetActive(true);
    }

    IEnumerator StartFirstValidation()
    {
        _fixationPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo save this time

        foreach (var element in _gridElementsClose)
        {
            _fixationPoint.transform.position = element.Position;

           yield return new WaitForSeconds(element.FixationDuration);
        }
        
        _gridClose.gameObject.transform.position = new Vector3(0, 0, 2);
        StartCoroutine(StartSecondValidation());
    }
    
    IEnumerator StartSecondValidation()
    {
        _fixationPoint.transform.position = new Vector3(0, 0, 2);
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo save this time
        
        foreach (var element in _gridElementsFar)
        {
            _fixationPoint.transform.position = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }

        _gridClose.gameObject.transform.position = new Vector3(0, 0, 1);
        ExperimentManager.Instance.TrialEnded();
    }

    public void StartValidation(List<GridElement> gridElementsClose, List<GridElement> gridElementsFar)
    {
        _gridElementsClose = gridElementsClose;
        _gridElementsFar = gridElementsFar;
        StartCoroutine(StartFirstValidation());
    }
}
