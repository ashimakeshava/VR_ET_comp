using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;


public class GridElementsGenerator : MonoBehaviour
{
    private GameObject _fixationPoint;
    private Random _random;

    private bool _isSmoothPursuit;

    
    private void Start()
    {
        _random = new Random();
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }
    
    private List<RaycastHit> GetHitList(float x = .2f, float y = .13f)
    {
        List<RaycastHit> hitList = new List<RaycastHit>();
        
        Vector3 halfExtents = new Vector3(x, y, .07f);

        var hits = Physics.BoxCastAll(_fixationPoint.transform.position, halfExtents, 
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

    public List<GridElement> Traverse(GameObject grid)
    {
        List<GridElement> gridElements = new List<GridElement>();
        int count = grid.transform.childCount;
        
        while (count != 0)
        {
            List<RaycastHit> hitList = GetHitList();

            if (!hitList.Any())
            {
                for (int i = 2; i < 5; i++)
                {
                    hitList= GetHitList(.2f*i, .13f*i);
                    
                    if (hitList.Any())
                    {
                        break;
                    }
                }
            }

            if (hitList.Any())
            {
                int index = _random.Next(hitList.Count);
            
                Vector3 newPosition = hitList[index].collider.transform.position;
            
                // TODO should move in a coroutine and according to the read time should fixate
                _fixationPoint.transform.position = newPosition;
            
                GridElement gridElement = new GridElement 
                {ObjectName = hitList[index].collider.name,
                    Position = newPosition,
                    FixationDuration = GenerateRandomFixationTime(),
                    MovementDuration = GenerateMovementTime()
                };

                hitList[index].collider.gameObject.SetActive(false);
                gridElements.Add(gridElement);
            }
            
            count--;
        }

        Debug.Log(gridElements.Count);
        return gridElements;
    }

    private float GenerateRandomFixationTime()
    {
        if (_isSmoothPursuit) return 1;
        else
            return (RandomUnity.value <= 0.5) ? 1 : 1.5f;
    }
    
    private float GenerateMovementTime()
    {
        return (_isSmoothPursuit) ? 2 : 0;
    }
}
