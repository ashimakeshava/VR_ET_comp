using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validation : MonoBehaviour
{
    private GameObject _fixationPoint;
    private GameObject _gridClose;
    private GameObject _gridFar;
    
    private List<GridElement> _gridElementsClose;
    private List<GridElement> _gridElementsFar;
    

    void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _gridClose = ExperimentManager.Instance.GetLargeGrid1();
        _gridFar = ExperimentManager.Instance.GetLargeGrid2();
        _gridClose.gameObject.SetActive(true);
    }

    IEnumerator StartFirstValidation()
    {
        foreach (var element in _gridElementsClose)
        {
            _fixationPoint.transform.position = element.Position;

           yield return new WaitForSeconds(element.FixationDuration);
        }

        _gridClose.gameObject.SetActive(false);
        _gridFar.gameObject.SetActive(true);
        StartCoroutine(StartSecondValidation());
    }
    
    IEnumerator StartSecondValidation()
    {
        _fixationPoint.transform.position = new Vector3(0, 0, 2);
        
        foreach (var element in _gridElementsFar)
        {
            _fixationPoint.transform.position = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }

        _gridFar.gameObject.SetActive(false);
        ExperimentManager.Instance.TrialEnded();
    }

    public void StartValidation(List<GridElement> gridElementsClose, List<GridElement> gridElementsFar)
    {
        _gridElementsClose = gridElementsClose;
        _gridElementsFar = gridElementsFar;
        StartCoroutine(StartFirstValidation());
    }
}
