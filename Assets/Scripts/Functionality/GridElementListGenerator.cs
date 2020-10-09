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
    private List<GridElement> _gridRoute;
    
    private List <List<GridElement>> _validGridRoutes;

    private bool _inValid;
    
    private void Start()
    {
        _random = new Random();
        _gridRoute = new List<GridElement>();
        _validGridRoutes = new List<List<GridElement>>();
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
        Vector3 oldPos;
        

        GridElement OldElement = new GridElement();
        _inValid = false;
        while (count > 0)
        {
            List<RaycastHit> hitList = GetHitList(_fixationPoint.transform.position);
            Debug.Log("count " + count );

            if (!hitList.Any())
            {
                Debug.Log("<color=blue> adjusting... </color>");
                for (int i = 2; i < 5; i++)
                {
                    Debug.Log("<color=blue> c:" + count + "i: " + i + "</color>");

                    hitList= GetHitList(_fixationPoint.transform.position,.2f*i, .13f*i);

                    if (hitList.Any())
                    {
                        break;
                    }
                    
                }
            }

            oldPos = _fixationPoint.transform.position;
            int index = _random.Next(hitList.Count);
            Vector3 newPosition = hitList[index].collider.transform.position;
            _fixationPoint.transform.position = newPosition;
            GridElement gridElement = new GridElement 
            {ObjectName = hitList[index].collider.name,
                Position = newPosition};
            
            hitList[index].collider.gameObject.SetActive(false);
            _gridRoute.Add(gridElement);

            
            Debug.Log(gridElement.ObjectName);
            Debug.Log(gridElement.Position);


            

            if (oldPos.x == newPosition.x)
            {
                if (Vector3.Distance(oldPos, newPosition) < 0.13)
                {
                    Debug.Log("<color=green>level 1 jump </color>" + "Old: " + OldElement.ObjectName+  "New: " + gridElement.ObjectName+  " distance :  " +Vector3.Distance(oldPos, newPosition));
                }else if (Vector3.Distance(oldPos, newPosition) < 0.26 )
                {
                    Debug.Log("<color=yellow>level 2 jump </color>" +  "Old: " + OldElement.ObjectName + "New: "+ gridElement.ObjectName+ " distance :  " +Vector3.Distance(oldPos, newPosition));
                }else if (Vector3.Distance(oldPos, newPosition) > 0.26)
                {
                    Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + OldElement.ObjectName + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                }
                else if (Vector3.Distance(oldPos, newPosition) > 0.45)
                {
                    Debug.Log("<color=red>level 4 jump </color>" + "Old: " + OldElement.ObjectName  + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                    _inValid = true;
                    Debug.Log("is invalid retry...");
                    break;
                }
            }
            else
            {
                if (Vector3.Distance(oldPos, newPosition) < 0.25)
                {
                    Debug.Log("<color=green>level 1 jump </color>" +  "Old: " + OldElement.ObjectName + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                }else if (Vector3.Distance(oldPos, newPosition) < 0.45 )
                {
                    Debug.Log("<color=yellow>level 2 jump </color>" +  "Old: " + OldElement.ObjectName  + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                }else if (Vector3.Distance(oldPos, newPosition) < 0.75)
                {
                    Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + OldElement.ObjectName  + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                }
                else if (Vector3.Distance(oldPos, newPosition) > 0.75)
                {
                    Debug.Log("<color=red>level 4 jump </color>" + "Old: " + OldElement.ObjectName  + "New: "+ gridElement.ObjectName + " distance :  " +Vector3.Distance(oldPos, newPosition));
                    _inValid = true;
                    Debug.Log("is invalid retry...");
                    count = 0;
                }
            }


            OldElement = gridElement;
            
            count--;
        }
        
        Debug.Log("is valid and can be checked with other grids");
        
        
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
    private void CheckWithOtherRoutes(List<GridElement> route )
    {
        if (!_validGridRoutes.Any())
        {
            return;
        }
        
        foreach (var validRoute in _validGridRoutes)
        {
            for (int i = 0; i < validRoute.Count; i++)
            {
                if (validRoute[i] == route[i])
                {
                    _inValid = true;
                }
            }
        }
    }
    public void GenerateGridElementList(GameObject grid)
    {
        int iter=0;
        _inValid = true;
        while (_inValid)
        {
            iter++;
            Debug.Log("iteration: " + iter);
            grid.gameObject.SetActive(true);
            Traverse(grid);
            if (_inValid)
                continue;
            
            CheckWithOtherRoutes(_gridRoute);
        }
        
        _validGridRoutes.Add(_gridRoute);
    }
}
