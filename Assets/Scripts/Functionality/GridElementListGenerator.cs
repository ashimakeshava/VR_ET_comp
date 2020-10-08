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
        RaycastHit[] hits;
        List<RaycastHit> hitList = new List<RaycastHit>();
        
        Vector3 _halfExtents = new Vector3(x, y, .07f);
        // Debug.Log("halfExtents: " + halfExtents);

        hits = Physics.BoxCastAll(position, _halfExtents, 
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


    IEnumerator Traverse(GameObject grid)
    {
        int count = grid.transform.childCount;

        while (count != 0)
        {
            List<RaycastHit> hitList= GetHitList(_fixationPoint.transform.position);

            if (!hitList.Any())
            {
                for (int i = 2; i < 5; i++)
                {
                    hitList= GetHitList(_fixationPoint.transform.position,.2f*i, .13f*i);
                    if (hitList.Any())
                    {
                        break;
                    }
                }

                break;
            }
            
            int index = _random.Next(hitList.Count);
            Vector3 newPosition = hitList[index].collider.transform.position;
            _fixationPoint.transform.position = newPosition;
            GridElement gridElement = new GridElement {Position = newPosition};
            _gridElements.Add(gridElement);
            
            count--;
        }

        yield return null;
    }
    
    public IEnumerator GetList(GameObject grid)
    {
        yield return Traverse(grid);
        
    }
    
    public List<GridElement> GetGridElements(GameObject grid)
    {
        StartCoroutine(GetList(grid));

         GetList(grid);
        return _gridElements;
    }

    public void GenerateGEL(GameObject grid)
    {
        StartCoroutine(GetList(grid));
    }
}
