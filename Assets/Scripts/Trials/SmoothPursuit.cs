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

    private float _t = 0f;
    private float _timeToReachGoal = 2f;
    
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetLargeGrid();    // todo change it to only grid after serialization is done
    }

    IEnumerator StartSmoothPursuit()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo generate this time as well or save this time

        for (int i = 0; i < _smoothPursuit.Count - 1; i++)
        {
            _t += Time.deltaTime / _timeToReachGoal;
            _fixationPoint.transform.position = Vector3.Lerp(_smoothPursuit[i].Position, _smoothPursuit[i + 1].Position, _t);
            
            yield return new WaitForSeconds(_smoothPursuit[i].FixationDuration);
        }

        ExperimentManager.Instance.TrialEnded();
    }

    public void RunSmoothPursuit(List<GridElement> smallGrid)
    {
        _smoothPursuit = smallGrid;
        StartCoroutine(StartSmoothPursuit());
    }
}
