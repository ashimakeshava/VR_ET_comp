using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System.Linq;


public class GridElementListGenerator : MonoBehaviour
{
    private GameObject _fixationPoint;
    private Random _random;
    private bool _isDone;
    private List<GridElement> _gridElements;
    
    private void Start()
    {
        _random = new Random();
        _gridElements = new List<GridElement>();
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }


    

    private List<RaycastHit> GetHitList(Vector3 position, float x=.2f, float y=.13f)
    {
        List<RaycastHit> hitList = new List<RaycastHit>();
        
        Vector3 halfExtents = new Vector3(x, y, .07f);

        var hits = Physics.BoxCastAll(position, halfExtents, 
            _fixationPoint.transform.forward, _fixationPoint.transform.rotation, 300f);
        
        foreach (var hit in hits)
        {
            if (hit.collider.name != "FixationPoint" && hit.collider.name != "LargeGrid(1)" 
                                                     && hit.collider.name != "LargeGrid(2)"
                                                     && hit.collider.name != "SmallGrid")
            {
                hitList.Add(hit);
            }
        }
        
        return hitList;
    }


    /*IEnumerator Traverse(GameObject grid)
    {
        int count = grid.transform.childCount;

        while (count != 0)
        {
            List<RaycastHit> hitList= GetHitList(_fixationPoint.transform.position);

            if (!hitList.Any())
            {
                Debug.Log("I'm empty");
                for (int i = 2; i < 5; i++)
                {
                    Debug.Log("i: " + i);

                    hitList= GetHitList(_fixationPoint.transform.position,.2f*i, .13f*i);
                    if (hitList.Any())
                    {
                        break;
                    }
                }

                // break;
            }
            
            int index = _random.Next(hitList.Count);
            Vector3 newPosition = hitList[index].collider.transform.position;
            _fixationPoint.transform.position = newPosition;
            GridElement gridElement = new GridElement 
            {ObjectName = hitList[index].collider.name,
                Position = newPosition};
            
            hitList[index].collider.gameObject.SetActive(false);
            _gridElements.Add(gridElement);

            count--;
        }
        
        foreach (var VARIABLE in _gridElements)
        {
            Debug.Log(VARIABLE.ObjectName);
            Debug.Log(VARIABLE.Position);
        }
        
        Debug.Log(_gridElements.Count);
        
        yield return null;
    }*/
    
    private void Traverse(GameObject grid)
    {
        int count = grid.transform.childCount;
        
        while (count != 0)
        {
            List<RaycastHit> hitList = GetHitList(_fixationPoint.transform.position);
            Debug.Log("count hit list" + hitList.Count);

            if (!hitList.Any())
            {
                for (int i = 2; i < 5; i++)
                {
                    Debug.Log("i: " + i);

                    hitList= GetHitList(_fixationPoint.transform.position,.2f*i, .13f*i);
                    Debug.Log("count hit list in for" + hitList.Count);
                    
                    if (hitList.Any())
                    {
                        break;
                    }
                }
            }
            
            int index = _random.Next(hitList.Count);
            Vector3 newPosition = hitList[index].collider.transform.position;
            _fixationPoint.transform.position = newPosition;
            GridElement gridElement = new GridElement 
            {ObjectName = hitList[index].collider.name,
                Position = newPosition};
            
            hitList[index].collider.gameObject.SetActive(false);
            _gridElements.Add(gridElement);
            
            Debug.Log(gridElement.ObjectName);
            Debug.Log(gridElement.Position);

            count--;
        }
        
        foreach (var VARIABLE in _gridElements)
        {
            Debug.Log(VARIABLE.ObjectName);
            Debug.Log(VARIABLE.Position);
        }
        
        Debug.Log(_gridElements.Count);
    }
    
    /*private IEnumerator GetElementList(GameObject grid)
    {
        yield return Traverse(grid);
    }
    
    private List<GridElement> GetGridElements(GameObject grid)
    {
        StartCoroutine(GetElementList(grid));
        return _gridElements;
    }*/

    public void GenerateGridElementList(GameObject grid)
    {
        Traverse(grid);
        // StartCoroutine(GetElementList(grid));
    }
}
