using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = System.Random;

// using Random = UnityEngine.Random;

public class RandomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fixationPoint;

    float m_MaxDistance;
    float m_Speed;

    Collider m_Collider;
    
    
    void Start()
    {
        //Choose the distance the Box can reach to
        m_MaxDistance = 300.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetNextPosition();
            Debug.Log("Clicked");
        }
    }

    void GetNextPosition()
    {
        Debug.Log("here");
        
        RaycastHit[] hits;
        List<RaycastHit> hitList = new List<RaycastHit>();
        Vector3 halfExtents = new Vector3(.2f, .13f, .07f);
        
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        
        hits = Physics.BoxCastAll(fixationPoint.transform.position, halfExtents, fixationPoint.transform.forward, fixationPoint.transform.rotation, 100f);

        foreach (var hit in hits)
        {
            if (hit.collider.name != "FixationPoint")
            {
                hitList.Add(hit);
            }
        }
        
        if (hitList.Any())
        {
            Random rand = new Random();
            int index = rand.Next(hitList.Count);
            
            fixationPoint.transform.position = hitList[index].collider.transform.position;
            hitList[index].collider.gameObject.SetActive(false);
            
            /*foreach (var hit in hits)
            {
                //Output the name of the Collider your Box hit
                Debug.Log("Hit : " + hit.collider.name);
            }*/
        }
        
        Debug.Log(hits.Length);
    }
    
    void FixedUpdate()
    {
        
    }
}
