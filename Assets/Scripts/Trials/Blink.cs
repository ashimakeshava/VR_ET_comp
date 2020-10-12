﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private GameObject _fixationPoint;
    private AudioSource _audioSource;

    private List<float> _delays;
    private GameObject _grid;


    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _audioSource = GetComponent<AudioSource>();
        _grid = ExperimentManager.Instance.GetLargeGrid();    // todo change it to only grid after serialization is done
    }

    IEnumerator RunBeep()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.position = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        foreach (var delay in _delays)
        {
            _audioSource.Play();
            yield return new WaitForSeconds(delay);
        }
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunBeepBlink(List<float> delays)
    {
        _delays = delays;
        StartCoroutine(RunBeep());
    }
}
