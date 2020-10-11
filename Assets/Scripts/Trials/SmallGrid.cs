using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

public class SmallGrid : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _grid;
    
    private List<GridElement> _smallGrid;
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetLargeGrid();
    }

    IEnumerator StartFirstValidation()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo generate this time as well or save this time

        foreach (var element in _smallGrid)
        {
            _fixationPoint.transform.position = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }
        
        _grid.gameObject.transform.position = new Vector3(0, 0, 2);
        ExperimentManager.Instance.TrialEnded();
    }

    public void StartValidation(List<GridElement> smallGrid)
    {
        _smallGrid = smallGrid;
        StartCoroutine(StartFirstValidation());
    }
}
