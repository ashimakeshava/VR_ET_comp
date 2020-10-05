﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSavingTriggerAutobahn : MonoBehaviour
{
    private GameObject _currentTarget;
    
    void Start()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _currentTarget)
            return;

        _currentTarget = other.gameObject;
        
        if (other.GetComponent<ManualController>() != null)
        {
            TimeManager.Instance.SetExperimentEndTime();
            // Debug.Log("<color=green>End of Experiment!</color>: ");
            SavingManager.Instance.StopAndSaveData("Autobahn");
        }
    }
}
