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
    private List <List<GridElement>> uniqueSmallGridRoutes;
    private List<List<GridElement>> uniqueLargeGridRoutse; // TODo maybe in another class?
    private List<List<GridElement>> uniqueSmoothPursuitRoutes;
    private bool _inValid;
    private int level1Jumps;
    private int level2Jumps;
    private int level3Jumps;
    private int level4Jumps;
    
    
    private void Start()
    {
        _random = new Random();
        _gridRoute = new List<GridElement>();
        //_validGridRoutes = new List<List<GridElement>>();
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
                                                     && hit.collider.name != "SmallGrid") //TODO check and change the names
            {
                hitList.Add(hit);
            }
        }
        
        return hitList;
    }

    private void Traverse(GameObject grid,int maximumJumpSize)
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
            int jumpsize=0;
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
                        jumpsize = 2;
                    }
                    if (i == 3)
                    {
                        level3Jumps++;
                        jumpsize = 3;
                    }
                    
                    if (i == 4)
                    {
                        level4Jumps++;
                        jumpsize = 4;
                    }

                    if (i > maximumJumpSize)
                    {
                        _inValid = true;
                        break;
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
                jumpsize = 1;
            }

            
            if(!_inValid)
            {
                int index = _random.Next(hitList.Count);
                Vector3 newPosition = hitList[index].collider.transform.position;
                _fixationPoint.transform.position = newPosition; //TODO this is only a Generator Script , movement happens in trail script
                GridElement gridElement = new GridElement
                {
                    ObjectName = hitList[index].collider.name,
                    Position = newPosition,
                    JumpSize =jumpsize,
                    PreviousObject = OldElement.ObjectName,
                    FixationDuration = GenerateRandomFixationTime(),
                    MovementDuration = GenerateMovementTime()
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

    }
    
    
    private void AddRouteToUniqueList(List<GridElement> route, List<List<GridElement>> uniqueRouteList, int familarityLevel)
    {
        bool same=false;
        if (!uniqueRouteList.Any())
        {
            uniqueRouteList.Add(route);
            return;
        }

        if (route.Count != uniqueRouteList.Count)
        {
            Debug.Log("the Routes have a different size, Route will not be added");
            
        }
        else
        {
            foreach (var gridRoute in uniqueRouteList)
            {
                int familarity = familarityLevel;
                for (int i = 0; i < gridRoute.Count; i++)
                {
                    if (route[i].Position == gridRoute[i].Position)
                    {
                        familarity--;
                    }

                    if (i + 1 < gridRoute.Count)
                    {
                        if (route[i].Position == gridRoute[i + 1].Position)
                        {
                            familarity--;
                        }
                        
                    }
                    if (familarity == 0)
                    {
                        Debug.Log("too familiar abort...");
                        return;
                    }
                }
                uniqueRouteList.Add(route);
            }
        }
    }
    
    private void GenerateGridRoute(GameObject grid, int allowedJumpSize=4)
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
            Traverse(grid, allowedJumpSize);

            if (_inValid)
            {
                Debug.Log("Retry, the Route is invalid");
                Debug.Log("Route: " + " level 1 Jumps: " + level1Jumps+ " level 2 Jumps: " + level2Jumps  + " level 3 Jumps: "+ level3Jumps + " level 4 Jumps: " + level4Jumps);
                _gridRoute.Clear();
            }
            else
            {
                Debug.Log("Route: " + " level 1 Jumps: " + level1Jumps+ " level 2 Jumps: " + level2Jumps  + " level 3 Jumps: "+ level3Jumps + " level 4 Jumps: " + level4Jumps);
            }
            
            
        } while (_inValid);
        
    }
    
    
    
    private float GenerateRandomFixationTime()        //TODO Add jitter and first position time: duration
    {
        if (_isSmoothPursuit) return 1;
        else
            return (RandomUnity.value <= 0.5) ? 1 : 1.5f;
    }


    public List<GridElement> GetGridRoute()
    {
        if (!_gridRoute.Any())
        {
            GenerateGridRoute(ExperimentManager.Instance.GetCurrentActiveGrid());
        }

        return _gridRoute;
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
