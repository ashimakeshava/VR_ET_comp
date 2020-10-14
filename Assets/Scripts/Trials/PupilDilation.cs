using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilDilation : MonoBehaviour
{
    [SerializeField] private Material black;
    [SerializeField] private List<Material> skyBoxes;
    
    private GameObject _fixationPoint;
    private float _blackFixationDuration;
    private List<PupilDilationDataFrame> _pupilDilationDataFrames;
    private GameObject _grid;

    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
        _grid = ExperimentManager.Instance.GetGrid();    // todo change it to only grid after serialization is done
    }

    IEnumerator StartPupilDilation()
    {
        _grid.gameObject.SetActive(true);
        _fixationPoint.transform.localPosition = Vector3.forward;
        _fixationPoint.gameObject.SetActive(true);
        
        RenderSettings.skybox = black;

        yield return new WaitForSeconds(_blackFixationDuration);

        foreach (var dataFrame in _pupilDilationDataFrames)
        {
            RenderSettings.skybox = skyBoxes[dataFrame.ColorIndex];
            yield return new WaitForSeconds(dataFrame.ColorDuration);
        }

        RenderSettings.skybox = ExperimentManager.Instance.GetMainSkybox();
        ExperimentManager.Instance.TrialEnded();
    }
    
    public void RunPupilDilation(List<PupilDilationDataFrame> dataFrames, float delay)
    {
        _blackFixationDuration = delay;
        _pupilDilationDataFrames = dataFrames;
        StartCoroutine(StartPupilDilation());
    }
}

// todo change the skybox back to default