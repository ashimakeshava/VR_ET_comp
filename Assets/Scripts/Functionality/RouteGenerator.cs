using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = System.Random;
using RandomUnity = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class RouteGenerator : MonoBehaviour
{
    private GameObject _fixationPoint;
    private Random _random;
    
    private bool _isDone;
    private bool _isSmoothPursuit;

    private List<GridElement> _gridRoute;
    [SerializeField] private List <List<GridElement>> uniqueSmallGridRoutes;
    private List<List<GridElement>> _uniqueGridRoutes; // TODo maybe in another class?
    private List<List<GridElement>> uniqueSmoothPursuitRoutes;

    private List<RouteFrame> _routeFrames;
    private RouteFrame _routeFrame;
    private bool _inValid;
    private int level1Jumps;
    private int level2Jumps;
    private int level3Jumps;
    private int level4Jumps;

    private List<PupilDilationDataFrame> _pupilDilationDataFrames;
    [ReadOnly] private readonly List<int> _pupilDilationSequence = new List<int> {0,1,2,3};

    private void Start()
    {
        _random = new Random();
        _gridRoute = new List<GridElement>();
        _uniqueGridRoutes = new List<List<GridElement>>();
        
        //uniqueSmallGridRoutes 
        //_validGridRoutes = new List<List<GridElement>>();
        _fixationPoint = ExperimentManager.Instance.GetFixationPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("oh mannn");
            GenerateUniqueRouteList(10, 3);

            List<RouteFrame> RouteFrameList = new List<RouteFrame>();

            foreach (var route in _uniqueGridRoutes)
            {
                RouteFrame routeFrame = new RouteFrame
                {
                    Route = route,
                    GridType = ExperimentManager.Instance.GetCurrentActiveGrid().name
                };
                RouteFrameList.Add(routeFrame);
            }
            
            SaveGridRoutes(RouteFrameList, "RouteList");
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            _routeFrame = new RouteFrame();
            _routeFrame = DataSavingManager.Instance.LoadFile<RouteFrame>("routeFrameLargeGrid 1");
            

            foreach (var elem in _routeFrame.Route)
            {
                Debug.Log(elem.ObjectName +  elem.Position);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            _routeFrames = DataSavingManager.Instance.LoadFileList<RouteFrame>("RouteListSmallGrid");

            int i= 0;
            foreach (var route in _routeFrames)
            {
                i++;
                Debug.Log("loaded route frame " + i + " "+ route.Route[0].ObjectName+","+ route.Route[1].ObjectName+"," + route.Route[2].ObjectName);
            }
            
        }
        
        
        

        if (Input.GetKeyDown(KeyCode.A))
        {
            List<GridElement> route= new List<GridElement>();
            Debug.Log("Draw Route");
            route= GetGridRoute(3);
            Color color = new Color(0,1,1,0);
            VisualizeRoute(route, color);
            Debug.Log(route.Count);

            RouteFrame routeFrame = new RouteFrame
            {
                Route = route,
                GridType = ExperimentManager.Instance.GetCurrentActiveGrid().gameObject.name
            };
            
            
            SaveGridRoute(routeFrame,"routeFrame");
        }
    }

    private void SaveGridRoute(RouteFrame route, string name)
    {
        DataSavingManager.Instance.Save(route, name+route.GridType);
    }

    private void SaveGridRoutes(List<RouteFrame> routes, string name)
    {
        DataSavingManager.Instance.SaveList(routes, name+routes[0].GridType);
    }

    private List<List<GridElement>> CheckForDoubles()
    {
        var _tmpRoute = new List<List<GridElement>>();
        foreach (var list in _uniqueGridRoutes)
        {
            _tmpRoute.Add(list);
        }
        
        foreach (var list in _tmpRoute)
        {
            for (int i = 0; i < _tmpRoute.Count; i++)
            {
                bool same=false;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Position == _tmpRoute[i][j].Position)
                    {
                        same = true;
                    }
                    else
                    {
                        same = false;
                        break;
                    }
                }

                if (same == true)
                {
                    _tmpRoute.RemoveAt(i);
                }
            }
            
        }

        return _tmpRoute;
    }


    private List<RaycastHit> GetHitList(float x=.2f, float y=.13f)
    {
        List<RaycastHit> hitList = new List<RaycastHit>();
        
        Vector3 halfExtents = new Vector3(x, y, .07f);

        var hits = Physics.BoxCastAll(_fixationPoint.transform.position, halfExtents, 
            _fixationPoint.transform.forward, _fixationPoint.transform.rotation, 300f);
        
        foreach (var hit in hits)
        {
            if (hit.collider.name != "FixationPoint" && hit.collider.name != "LargeGrid" 
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

        _fixationPoint.transform.position = Vector3.forward;


        GridElement OldElement = new GridElement{
            ObjectName = _fixationPoint.gameObject.name,
            Position = _fixationPoint.transform.position,
            FixationDuration = GenerateRandomFixationTime(),
            MovementDuration = GenerateMovementTime()
            };
       
        _gridRoute.Add(OldElement);
        
        _fixationPoint.gameObject.SetActive(false);
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

                
                
                LogJump(OldElement, gridElement);
                OldElement = gridElement;
                
                count--;
                
                
            }
            else
            {
                count=0;
                //Debug.Log("invalid");
            }
        }
       // Debug.Log("Route starts with: " +_gridRoute[1].ObjectName+ " "+ _gridRoute[2].ObjectName+" "+ _gridRoute[3].ObjectName);

    }

    private void GenerateUniqueRouteList(int amountOfRoutes, int jumpsize=4)
    {
        int iter=0;
        int OutOfBounce = 10000;
        int routeListCount=0;
        List<List<GridElement>> tmpRouteList = new List<List<GridElement>>();
        
        //tmpRouteList = routeList;
        while (routeListCount<=amountOfRoutes&&iter<OutOfBounce)
        {
            
            iter++;
            Debug.Log("<color=blue> iteration of trying to Add route"+ iter +"</color>" );
            
            List<GridElement> tmpRoute = new List<GridElement>();
            
            GetGridRoute(jumpsize);

            foreach (var elem in _gridRoute)
            {
                tmpRoute.Add(elem);
            }
            _gridRoute.Clear();
            
            AddRouteToUniqueRouteList(tmpRoute, _gridRoute.Capacity);

            tmpRouteList = _uniqueGridRoutes;
            
            routeListCount = tmpRouteList.Count;
            
        }

        _uniqueGridRoutes = tmpRouteList;

        Debug.Log("found : " + routeListCount + "in " + iter + "tries");

        for (int i = 0; i < tmpRouteList.Count; i++)
        {
            Color color = new Color(0,1,1,0);
            VisualizeRoute(tmpRouteList[i],color,Vector3.forward*i);
        }
        
    }
    private void AddRouteToUniqueRouteList(List<GridElement> route, int familarityLevel=11)
    {
        bool same=false;
        if (!_uniqueGridRoutes.Any())
        {
            _uniqueGridRoutes.Add(route);
            Debug.Log("added a route... "+ route[0].ObjectName+ " "+route[1].ObjectName+ " "+  route[2].ObjectName);
        }else
        {
            foreach (var gridRoute in _uniqueGridRoutes)
            {
                if (route.Count != gridRoute.Count)
                {
                    Debug.Log("the Routes have a different size, Route will not be added");
                }
                
                int familarity = familarityLevel;
                
                for (int i = 0; i < gridRoute.Count; i++)
                {
                    if (route[i].Position == gridRoute[i].Position)
                    {
                        familarity--;
//                        Debug.Log(route[i].ObjectName +"... interesting... sounds familiar");
                    }

                    if (i + 1 < gridRoute.Count)         //something to test if the following numbers are shifted but similar
                    {
                        if (route[i].Position == gridRoute[i + 1].Position)
                        {
                            familarity--;
                       //     Debug.Log(gridRoute[i].ObjectName +"... interesting... sounds familiar..." + "I got here" +route[i]);
                        }
                        
                    }
                    if (familarity == 0)
                    {
                        Debug.Log("too familiar abort...");
                        return;
                    }
                }
                Debug.Log("with GridRoute " + gridRoute + "degree of familarity :" + familarity);
            }
        }
        _uniqueGridRoutes.Add(route);
    }
    
    
    private void GenerateGridRoute(GameObject grid, int allowedJumpSize=4)
    {
        int iter=0;
        int overFlow=10000;
        do
        {
            iter++;
            level1Jumps = 0;
            level2Jumps = 0;
            level3Jumps = 0;
            level4Jumps = 0;
            Debug.Log("<color=red> iteration of Routegeneration  "+ iter + "</color> ");
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
            
            
        } while (_inValid&&iter!=overFlow);

        if (iter == overFlow)
        {
            Debug.Log("overflow error");
        }
        
    }
    
    
    
    private float GenerateRandomFixationTime()        //TODO Add jitter and first position time: duration
    {
        if (_isSmoothPursuit) return 1;
        else
            return ((RandomUnity.value <= 0.5) ? 1 : 1.5f) + (RandomUnity.Range(-.2f, .2f));
    }


    public List<GridElement> GetGridRoute(int jumpsize=4)
    {
        if (!_gridRoute.Any())
        {
            Debug.Log("<color=orange>Create new one </color>");
            GenerateGridRoute(ExperimentManager.Instance.GetCurrentActiveGrid(),jumpsize);
        }
        else
        {
            Debug.Log("<color=yellow>I give you the last </color>");
        }

        return _gridRoute;
    }
    private float GenerateMovementTime()
    {
        return (_isSmoothPursuit) ? 2 : 0;
    }

    private void VisualizeRoute(List<GridElement> Route, Color color, Vector3 offset = default(Vector3))
    {

        //Debug.Log(Route.Count);
        Color runningColors=color;
        for (int i = 0; i< Route.Count; i++)
        {
            
            GameObject sphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.gameObject.name = Route[i].ObjectName + " " +i;
            sphere.transform.localScale= Vector3.one*0.05f;
            sphere.transform.position= (Route[i].Position)+offset;
            sphere.GetComponent<MeshRenderer>().material.color = Color.white*0.001f+ runningColors;

            if (i + 1 < Route.Count)
            {
                var lineRenderer = sphere.AddComponent<LineRenderer>();
                lineRenderer.material.color = color;
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0,Route[i].Position+offset);
                lineRenderer.SetPosition(1,Route[i+1].Position+offset);
                //sphere.GetComponent<MeshRenderer>().material.color = Color.white*0.001f+ color*(0.05f*i);
                //Debug.DrawLine(Route[i].Position,Route[i+1].Position, color*(0.010f*i),2.5f);
            }

            runningColors = runningColors* 0.90f;
        }
       
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

    public List<T> RandomiseSequence<T>(List<T> listToRand)
    {
        List<T> list = new List<T>();
        
        foreach (var item in listToRand)
        {
            int index = RandomUnity.Range(0, listToRand.Count);
            
            list.Add(item);
            listToRand.Remove(item);
        }
        
        return list;
    }

    public List<PupilDilationDataFrame> RandomisePupilDilationDataFrame()
    {
        List<PupilDilationDataFrame> pupilDilationDataFrames = new List<PupilDilationDataFrame>();
        
        for (int i = 0; i < _pupilDilationSequence.Count; i++)
        {
            int index = _random.Next(_pupilDilationSequence.Count);
            
            pupilDilationDataFrames[i].ColorIndex = index;
            pupilDilationDataFrames[i].ColorDuration = 3f + RandomUnity.Range(-.2f, .2f);

            _pupilDilationSequence.RemoveAt(index);
        }

        return pupilDilationDataFrames;
    }
}
