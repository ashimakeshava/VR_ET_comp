using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

public class Validation : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _grid;
    
    private List<GridElement> _gridElementsClose;
    private List<GridElement> _gridElementsFar;
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetGrid();
    }

    IEnumerator StartFirstValidation()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.zero;
        _fixationPoint.gameObject.SetActive(true);
        
        ExperimentManager.Instance.SetFixationPointActivationStatus(true);
        ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

        foreach (var element in _gridElementsClose)
        {
            _fixationPoint.transform.localPosition = element.Position;
            ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

            yield return new WaitForSeconds(element.FixationDuration);
        }
        
        _grid.gameObject.transform.position = new Vector3(0, 0, 2);
        
        yield return new WaitForSeconds(2f);
            
        StartCoroutine(StartSecondValidation());
    }
    
    IEnumerator StartSecondValidation()
    {
        _fixationPoint.transform.localPosition = Vector3.zero;
        ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

        foreach (var element in _gridElementsFar)
        {
            _fixationPoint.transform.localPosition = element.Position;
            ExperimentManager.Instance.SetFixationPointPosition(_fixationPoint.transform.position);

            yield return new WaitForSeconds(element.FixationDuration);
        }

        _grid.gameObject.transform.position = new Vector3(0, 0, 1);
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunValidation(List<GridElement> gridElementsClose, List<GridElement> gridElementsFar)
    {
        _gridElementsClose = gridElementsClose;
        _gridElementsFar = gridElementsFar;
        StartCoroutine(StartFirstValidation());
    }
}
