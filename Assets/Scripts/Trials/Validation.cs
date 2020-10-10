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
    }

    IEnumerator StartFirstValidation()
    {
        foreach (var element in _gridElementsClose)
        {
            _fixationPoint.transform.position = element.Position;

           yield return new WaitForSeconds(element.FixationDuration);
        }

        StartCoroutine(StartSecondValidation());
    }
    
    IEnumerator StartSecondValidation()
    {
        foreach (var element in _gridElementsFar)
        {
            _fixationPoint.transform.position = element.Position;

            yield return new WaitForSeconds(element.FixationDuration);
        }

        ExperimentManager.Instance.TrialEnded();
    }

    public void StartValidation(List<GridElement> gridElementsClose, List<GridElement> gridElementsFar)
    {
        _gridElementsClose = gridElementsClose;
        _gridElementsFar = gridElementsFar;
        StartCoroutine(StartFirstValidation());
    }
}
