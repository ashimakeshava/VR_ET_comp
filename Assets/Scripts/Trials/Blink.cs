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

    public delegate void OnsetStimuli();
    public event OnsetStimuli NotifyStimuliObservers;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _audioSource = GetComponent<AudioSource>();
        _grid = ExperimentManager.Instance.GetGrid();
    }

    IEnumerator RunBeep()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.zero;
        _fixationPoint.gameObject.SetActive(true);
        ExperimentManager.Instance.SetFixationPointActivationStatus(true);
        ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);
        ExperimentManager.Instance.SetFixationPointLocalPosition(_fixationPoint.transform.localPosition);
        
        yield return new WaitForSeconds(_delays[0]);
        _delays.RemoveAt(0);
        
        foreach (var delay in _delays)
        {
            _audioSource.Play();
            NotifyStimuliObservers?.Invoke();
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
