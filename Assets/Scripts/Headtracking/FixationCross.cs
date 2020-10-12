using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationCross : MonoBehaviour
{
    [SerializeField] private GameObject Upper;
    [SerializeField] private GameObject Lower;
    [SerializeField] private GameObject Left;
    [SerializeField] private GameObject Right;
    [SerializeField] private GameObject Center;
    
    [SerializeField] private GameObject TargetUpper;
    [SerializeField] private GameObject TargetLower;
    [SerializeField] private GameObject TargetLeft;
    [SerializeField] private GameObject TargetRight;
    [SerializeField] private GameObject TargetCenter;
    
    
    
    [SerializeField] private Material[] matArray;
    private Material error;
    private Material sucess;
    private Material neutral;
    private List<GameObject> CrossElements;
    
    private List<GameObject> TargetCrossElements;

    private GameObject TargetObject;
    private bool Oriented;
    private bool isAligned;
    
    // Start is called before the first frame update
    void Start()
    {
      // matArray = Center.GetComponent<Renderer>().materials;
      // neutral = matArray[0];
       // error = matArray[1];
        //sucess = matArray[2];
        
        CrossElements= new List<GameObject>();
        
        TargetCrossElements= new List<GameObject>();
        
        CrossElements.Add(Upper);
        CrossElements.Add(Lower);    
        CrossElements.Add(Left);    
        CrossElements.Add(Right);    
        CrossElements.Add(Center);
        
        TargetCrossElements.Add(TargetUpper);
        TargetCrossElements.Add(TargetLower);    
        TargetCrossElements.Add(TargetLeft);    
        TargetCrossElements.Add(TargetRight);    
        TargetCrossElements.Add(TargetCenter);
        
    
    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < CrossElements.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(CrossElements[i].transform.position, Vector3.forward, out hit, 50.0f))
            {
                if (hit.collider.gameObject==TargetCrossElements[i])
                {
                    CrossElements[i].GetComponent<Renderer>().material.color = Color.green*0.6f ;
                    isAligned = true;
                }
                else
                {
                    CrossElements[i].GetComponent<Renderer>().material.color = Color.red*0.6f;
                    isAligned = false;
                }
            }
            else
            {
                CrossElements[i].GetComponent<Renderer>().material.color = Color.grey *  0.6f;
            }
        }

        if (isAligned)
        {
            Debug.Log("Cross is aligned");
        }
        
        
    }


    private void ResetCross()
    {
        foreach (var cross in CrossElements)
        {
            cross.gameObject.SetActive(true);
        }
    }
    
    public void DisableVertical()
    {
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
    }
    
    
    public void DisableHorizontal()
    {
        Upper.gameObject.SetActive(false);
        Lower.gameObject.SetActive(false);
    }


    public void SetTargetObject(GameObject target)
    {
        TargetObject = target;
    }

    public bool GetAlignment()
    {
        return isAligned;
    }
}
