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
    
    private void Start()
    {
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }

    IEnumerator StartPupilDilation()
    {
        _fixationPoint.transform.position = new Vector3(0,0,1);
        _fixationPoint.gameObject.SetActive(true);
        
        RenderSettings.skybox = black;

        yield return new WaitForSeconds(_blackFixationDuration);

        foreach (var dataFrame in _pupilDilationDataFrames)
        {
            RenderSettings.skybox = skyBoxes[dataFrame.ColorIndex];
            yield return new WaitForSeconds(dataFrame.ColorDuration);
        }
        
        ExperimentManager.Instance.TrialEnded();
    }
    
    public void RunPupilDilation(List<PupilDilationDataFrame> dataFrames, float delay)
    {
        _blackFixationDuration = delay;
        _pupilDilationDataFrames = dataFrames;
        StartCoroutine(StartPupilDilation());
    }
}
