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
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(_gridElementsClose[0].FixationDuration);    // todo save this time

        foreach (var element in _gridElementsClose)
        {
            _fixationPoint.transform.localPosition = element.Position;

           yield return new WaitForSeconds(element.FixationDuration);
        }
        
        _grid.gameObject.transform.localPosition = new Vector3(0, 0, 2);
        
        yield return new WaitForSeconds(2f);
            
        StartCoroutine(StartSecondValidation());
    }
    
    IEnumerator StartSecondValidation()
    {
        _fixationPoint.transform.localPosition = new Vector3(0, 0, 2);
        
        yield return new WaitForSeconds((RandomUnity.value <= 0.5) ? 1 : 1.5f);    // todo save this time
        
        foreach (var element in _gridElementsFar)
        {
            _fixationPoint.transform.localPosition = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }

        _grid.gameObject.transform.localPosition = new Vector3(0, 0, 1);
        ExperimentManager.Instance.TrialEnded();
    }

    public void RunValidation(List<GridElement> gridElementsClose, List<GridElement> gridElementsFar)
    {
        _gridElementsClose = gridElementsClose;
        _gridElementsFar = gridElementsFar;
        StartCoroutine(StartFirstValidation());
    }
}
