using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

public class SmallGrid : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _grid;
    
    private List<GridElement> _smallGridElements;
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetGrid();    // todo change it to only grid after serialization is done
    }

    IEnumerator StartSmallGrid()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.position = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo generate this time as well or save this time

        foreach (var element in _smallGridElements)
        {
            _fixationPoint.transform.position = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }
        
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunSmallGrid(List<GridElement> smallGrid)
    {
        _smallGridElements = smallGrid;
        StartCoroutine(StartSmallGrid());
    }
}
