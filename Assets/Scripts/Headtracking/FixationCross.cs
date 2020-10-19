using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationCross : MonoBehaviour
{
    [SerializeField] private GameObject Upper0;
    [SerializeField] private GameObject Lower0;
    [SerializeField] private GameObject Left0;
    [SerializeField] private GameObject Right0;
    [SerializeField] private GameObject Center;
    [SerializeField] private GameObject Upper1;
    [SerializeField] private GameObject Lower1;
    [SerializeField] private GameObject Left1;
    [SerializeField] private GameObject Right1;

    [SerializeField] private GameObject TargetUpper0;
    [SerializeField] private GameObject TargetUpper1;
    [SerializeField] private GameObject TargetLower0;
    [SerializeField] private GameObject TargetLower1;
    [SerializeField] private GameObject TargetLeft0;
    [SerializeField] private GameObject TargetLeft1;
    [SerializeField] private GameObject TargetRight0;
    [SerializeField] private GameObject TargetRight1;
    [SerializeField] private GameObject TargetCenter;

    [SerializeField] private GameObject GlobalSphere;

    private Material error;
    private Material sucess;
    private Material neutral;
    private List<GameObject> CrossElements;
    
    private List<GameObject> TargetCrossElements;

    private GameObject TargetObject;
    private bool Oriented;
    private bool isAligned;
    private bool isHorizontal;
    private bool isVertical;
    private bool isReduced;

    private List<bool> _alignmentStatus;
    
    void Start()
    {
        CrossElements= new List<GameObject>();
        _alignmentStatus = new List<bool>();
        
        TargetCrossElements= new List<GameObject>();
        
        CrossElements.Add(Upper0);
        CrossElements.Add(Upper1);
        CrossElements.Add(Lower0);
        CrossElements.Add(Lower1);
        CrossElements.Add(Left0);
        CrossElements.Add(Left1);    
        CrossElements.Add(Right0);
        CrossElements.Add(Right1);  
        CrossElements.Add(Center);
        
        TargetCrossElements.Add(TargetUpper0);
        TargetCrossElements.Add(TargetUpper1);
        TargetCrossElements.Add(TargetLower0);
        TargetCrossElements.Add(TargetLower1);    
        TargetCrossElements.Add(TargetLeft0);
        TargetCrossElements.Add(TargetLeft1);
        TargetCrossElements.Add(TargetRight0);
        TargetCrossElements.Add(TargetRight1);
        TargetCrossElements.Add(TargetCenter);
    }

    void FixedUpdate()
    {
        isAligned = false;
            
        foreach (var element in CrossElements)
        {
            if (element.activeInHierarchy)
            {
                RaycastHit[] hits = Physics.BoxCastAll(element.transform.position,
                    element.transform.lossyScale / 1.8f,
                    element.transform.forward,
                    element.transform.rotation, 100f,1);
            
                bool catched = false;
            
                if (hits.Length == 0)
                {
                    element.GetComponent<Renderer>().material.color = Color.grey *  0.6f;
                    _alignmentStatus.Add(catched);
                    continue;
                }
            
                foreach (var hit in hits)
                {
                    // Todo check if ignore global fixation point layer breaks eye tracking raycast
                    // Todo on the large grid (the most important part of the experiment)
            
                    if (hit.collider.gameObject == element.GetComponent<CrossElement>().targetCrossElement || TargetObject)
                    {
                        element.GetComponent<Renderer>().material.color = Color.green * 0.6f;
                        element.GetComponent<CrossElement>().correctAligned = true;
                        catched = true;
                        break;
                    }
                
                    element.GetComponent<Renderer>().material.color = Color.red * 0.6f;
                }
            
                _alignmentStatus.Add(catched);
            }
        }
        
        foreach (var value in _alignmentStatus)
        {
            if (!value)
            {
                isAligned = false;
                break;
            }
            
            isAligned = true;
        }
    
        Debug.Log(isAligned);
        
        _alignmentStatus.Clear();
    }
    
    public void ResetCross()
    {
        foreach (var cross in CrossElements)
        {
            cross.gameObject.SetActive(true);
        }

        isReduced = false;
    }
    
    public void DisableVertical()
    {
        ResetCross();
        Left0.gameObject.SetActive(true);
        Left1.gameObject.SetActive(true);
        
        Right0.gameObject.SetActive(true);
        Right1.gameObject.SetActive(true);

        Upper0.gameObject.SetActive(false);
        Upper1.gameObject.SetActive(false);
        
        Lower0.gameObject.SetActive(false);
        Lower1.gameObject.SetActive(false);

        isAligned = false;
        isReduced = true;
    }
    
    public void DisableHorizontal()
    {
        ResetCross();
        Left0.gameObject.SetActive(false);
        Left1.gameObject.SetActive(false);
        
        Right0.gameObject.SetActive(false);
        Right1.gameObject.SetActive(false);
        
        Upper0.gameObject.SetActive(true);
        Upper1.gameObject.SetActive(true);
        
        Lower0.gameObject.SetActive(true);
        Lower1.gameObject.SetActive(true);
        
        isAligned = false;
        isReduced = true;
    }
    
    public void SetTargetObject(GameObject target)
    {
        TargetObject = target;
    }

    public bool GetAlignment()
    {
        return isAligned;
    }

    public void SetAlignmentStatus(bool alignment)
    {
        isAligned = alignment;
    }
}
