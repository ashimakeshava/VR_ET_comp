using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private GameObject _fixationPoint;
    private AudioSource _audioSource;

    private List<float> _delays;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _audioSource = GetComponent<AudioSource>();
    }

    IEnumerator RunBeep()
    {

        foreach (var delay in _delays)
        {
            _audioSource.Play();
            yield return new WaitForSeconds(delay);
        }
    }

    public void RunBeepBlink(List<float> delays)
    {
        _delays = delays;
        _fixationPoint.transform.position = new Vector3(0, 0, 1);
        _fixationPoint.gameObject.SetActive(true);
        StartCoroutine(RunBeep());
    }
}
