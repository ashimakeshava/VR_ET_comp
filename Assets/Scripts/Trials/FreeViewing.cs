using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeViewing : MonoBehaviour
{
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
            _fixationPoint.transform.position = new Vector3(0,0,1);
            _fixationPoint.gameObject.SetActive(true);
        
            yield return new WaitForSeconds(frame.FixationPointDuration);
            
            _fixationPoint.gameObject.SetActive(false);
            frame.Picture.SetActive(true);

            yield return new WaitForSeconds(frame.PhotoFixationDuration);
            
            frame.Picture.SetActive(false);
        }
    }

    public void RunFreeViewing(List<FreeViewingDataFrame> dataFrames)
    {
        _freeViewingDataFrames = dataFrames;
        StartCoroutine(StartFreeViewing());
    }
}
