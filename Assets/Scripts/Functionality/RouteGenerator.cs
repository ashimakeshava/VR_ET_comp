using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;


public class RouteGenerator : MonoBehaviour
{
    private GameObject _fixationPoint;
    private Random _random;
    
    private bool _isDone;
    private bool _isSmoothPursuit;

    private List<GridElement> _gridRoute;
    private List <List<GridElement>> _validGridRoutes; // TODo maybe in another class?

    private bool _inValid;
    private int level1Jumps;
    private int level2Jumps;
    private int level3Jumps;
    private int level4Jumps;
    
    private void Start()
    {
        _random = new Random();
        _gridRoute = new List<GridElement>();
        _validGridRoutes = new List<List<GridElement>>();
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }

    
    private List<RaycastHit> GetHitList(float x=.2f, float y=.13f)
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

    private void Traverse(GameObject grid)
    {
        int count = grid.transform.childCount;
        Vector3 oldPos;


        GridElement OldElement = new GridElement{
            ObjectName = _fixationPoint.gameObject.name,
            Position = _fixationPoint.transform.position
            };
        _inValid = false;
        while (count > 0)
        {
            List<RaycastHit> hitList = GetHitList();
            //Debug.Log("fixation " + _fixationPoint.transform.position );

            if (!hitList.Any())
            {
                //Debug.Log("<color=blue> adjusting... </color>");
                for (int i = 2; i < 5; i++)
                {
                    //Debug.Log("<color=blue> c:" + count + "i: " + i + "</color>");
                    if (i == 2)
                    {
                        level2Jumps++;
                    }
                    if (i == 3)
                    {
                        level3Jumps++;
                    }
                    
                    if (i == 4)
                    {
                        _inValid = true;
                        level4Jumps++;
                    }
                    
                    hitList= GetHitList(.2f*i, .13f*i);
                    
                    

                    if (hitList.Any())
                    {
                        break;
                    }

                    
                }
            }
            else
            {
                level1Jumps++;
            }

            
            if(!_inValid)
            {
                oldPos = _fixationPoint.transform.position;
                int index = _random.Next(hitList.Count);
                Vector3 newPosition = hitList[index].collider.transform.position;
                _fixationPoint.transform.position = newPosition;
                GridElement gridElement = new GridElement
                {
                    ObjectName = hitList[index].collider.name,
                    Position = newPosition
                };

                hitList[index].collider.gameObject.SetActive(false);
                _gridRoute.Add(gridElement);

                
                //LogJump(OldElement, gridElement);
                
                OldElement = gridElement;
                
                count--;
            }
            else
            {
                count=0;
                Debug.Log("invalid");
            }
        }
        //grid.SetActive(true);

        if (!_inValid)
        {
            Debug.Log("is valid and can be checked with other grids"); 
        }
        else
        {
            Debug.Log("needs to be rerouted"); 
        }
    }
    
    
    private void CheckWithOtherRoutes(List<GridElement> route )
    {
        bool same=false;
        if (!_validGridRoutes.Any())
        {
        }
        else
        {
            foreach (var gridRoute in _validGridRoutes)
            {
                for (int i = 0; i < gridRoute.Count; i++)
                {
                    if (route[i] == gridRoute[i])
                    {
                        same = true;
                    }
                    else
                    {
                        same = false;
                    }
                }
            }
        }
    }
    public void GenerateGridRoute(GameObject grid)
    {
        int iter=0;
        do
        {
            iter++;
            level1Jumps = 0;
            level2Jumps = 0;
            level3Jumps = 0;
            level4Jumps = 0;
            Debug.Log("<color=red> iteration  "+ iter + "</color> ");
            for (int i = 0; i < grid.transform.childCount;i++)
            {
                grid.transform.GetChild(i).gameObject.SetActive(true);

            }
            Traverse(grid);

            if (_inValid)
            {
                Debug.Log("Retry, the Route is invalid");
                Debug.Log("Route: " + " level 1 Jumps: " + level1Jumps+ " level 2 Jumps: " + level2Jumps  + " level 3 Jumps: "+ level3Jumps + " level 4 Jumps: " + level4Jumps);
            }
            else
            {
                Debug.Log("Route: " + " level 1 Jumps: " + level1Jumps+ " level 2 Jumps: " + level2Jumps  + " level 3 Jumps: "+ level3Jumps + " level 4 Jumps: " + level4Jumps);
            }
            
            
        } while (_inValid);
        
        
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
    
    private void LogJump(GridElement oldPos, GridElement newPos)
    {
        if (oldPos.Position.x == newPos.Position.x)
                {
                    if (Vector3.Distance(oldPos.Position, newPos.Position) < 0.13)
                    {
                        Debug.Log("<color=green>level 1 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) < 0.26)
                    {
                        Debug.Log("<color=yellow>level 2 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) > 0.26)
                    {
                        Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) > 0.45)
                    {
                        Debug.Log("<color=red>level 4 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                        Debug.Log("is invalid retry...");
                    }
                }
                else
                {
                    if (Vector3.Distance(oldPos.Position, newPos.Position) < 0.25)
                    {
                        Debug.Log("<color=green>level 1 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) < 0.45)
                    {
                        Debug.Log("<color=yellow>level 2 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) < 0.75)
                    {
                        Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                    }
                    else if (Vector3.Distance(oldPos.Position, newPos.Position) > 0.75)
                    {
                        Debug.Log("<color=red>level 4 jump </color>" + "Old: " + oldPos.ObjectName + "New: " +
                                  newPos.ObjectName + " distance :  " + Vector3.Distance(oldPos.Position, newPos.Position));
                        Debug.Log("is invalid retry...");
                    }
                }
    }
}
