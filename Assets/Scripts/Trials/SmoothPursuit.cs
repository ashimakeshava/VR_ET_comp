using System.Collections;
using System.Collections.Generic;
using Tobii.StreamEngine;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

public class SmoothPursuit : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _grid;
    
    private List<GridElement> _smoothPursuit;

    private float _timeToReachGoal = 2.5f;

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetGrid();    // todo change to only collider since at the runtime we won't have large and small grid
    }

    IEnumerator StartSmoothPursuit()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        float startTime = Time.time;

        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo generate this time as well or save this time

        for (int i = 1; i < _smoothPursuit.Count; i++)
        {
            startTime = Time.time;
            float timeDiff = 0;
            while (timeDiff < _timeToReachGoal)
            {
                _fixationPoint.transform.localPosition = Vector3.Lerp(_smoothPursuit[i - 1].Position, _smoothPursuit[i].Position, timeDiff / 1f);
                
                yield return new WaitForEndOfFrame();

                
                timeDiff = Time.time - startTime;
            }

            yield return new WaitForSeconds(_smoothPursuit[i].FixationDuration);
        }

        ExperimentManager.Instance.TrialEnded();
    }

    public void RunSmoothPursuit(List<GridElement> route)
    {
        _smoothPursuit = route;
        StartCoroutine(StartSmoothPursuit());
    }
}
