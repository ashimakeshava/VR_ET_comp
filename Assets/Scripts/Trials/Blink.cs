using System;
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
        _grid = ExperimentManager.Instance.GetGrid();
    }

    IEnumerator RunBeep()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(_delays[0]);
        _delays.RemoveAt(0);
        
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
