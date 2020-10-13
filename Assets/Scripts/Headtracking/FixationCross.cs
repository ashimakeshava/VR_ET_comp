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

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CrossElements.Count);
        for (int i = 0; i < CrossElements.Count; i++)
        {
            Debug.DrawRay(CrossElements[i].transform.position,CrossElements[i].transform.forward);
            
            
            RaycastHit[] hits = Physics.BoxCastAll(CrossElements[i].transform.position,
                CrossElements[i].transform.lossyScale / 1.8f,
                this.CrossElements[i].transform.forward,
                CrossElements[i].transform.rotation, 100f,1);

            //Debug.Log(hits.Length +" " +CrossElements[i]);

            foreach (var hit in hits)
            {
                Debug.DrawLine(CrossElements[i].transform.position,hit.transform.position);
            }
            
            bool catched=false;
            
            if (hits.Length == 0)
            {
                CrossElements[i].GetComponent<Renderer>().material.color = Color.grey *  0.6f;
                continue;
            }
            
            foreach (var hit in hits)
            {
                
                //Debug.Log("hits.Length" + hit.transform.gameObject.name+ "+ "+ CrossElements[i]);
                
                if (hit.collider.gameObject == TargetCrossElements[i] || TargetObject)
                {
                    CrossElements[i].GetComponent<Renderer>().material.color = Color.green * 0.6f;
                    catched = true;
                }
                else
                {
                    if (catched)
                    {
                        continue;
                    }
                    CrossElements[i].GetComponent<Renderer>().material.color = Color.red * 0.6f;
                    Debug.DrawLine(CrossElements[i].transform.position,hit.transform.position,Color.red);
                }

                if (catched)
                {
                    isAligned = true;
                }
               
            }
            

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
        ResetCross();
        Left0.gameObject.SetActive(true);
        Left1.gameObject.SetActive(true);
        
        Right0.gameObject.SetActive(true);
        Right1.gameObject.SetActive(true);


        Upper0.gameObject.SetActive(false);
        Upper1.gameObject.SetActive(false);
        
        Lower0.gameObject.SetActive(false);
        Lower1.gameObject.SetActive(false);
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
