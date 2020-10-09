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
        

        GridElement OldElement = new GridElement();
        _inValid = false;
        while (count > 0)
        {
            List<RaycastHit> hitList = GetHitList();
            Debug.Log("count " + count );

            if (!hitList.Any())
            {
                Debug.Log("<color=blue> adjusting... </color>");
                for (int i = 2; i < 5; i++)
                {
                    Debug.Log("<color=blue> c:" + count + "i: " + i + "</color>");

                    hitList= GetHitList(.2f*i, .13f*i);

                    if (hitList.Any())
                    {
                        break;
                    }

                    if (i == 4)
                    {
                        _inValid = true;
                    }
                }
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


                Debug.Log(gridElement.ObjectName);
                Debug.Log(gridElement.Position);




                if (oldPos.x == newPosition.x)
                {
                    if (Vector3.Distance(oldPos, newPosition) < 0.13)
                    {
                        Debug.Log("<color=green>level 1 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) < 0.26)
                    {
                        Debug.Log("<color=yellow>level 2 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) > 0.26)
                    {
                        Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) > 0.45)
                    {
                        Debug.Log("<color=red>level 4 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                        _inValid = true;
                        Debug.Log("is invalid retry...");
                    }
                }
                else
                {
                    if (Vector3.Distance(oldPos, newPosition) < 0.25)
                    {
                        Debug.Log("<color=green>level 1 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) < 0.45)
                    {
                        Debug.Log("<color=yellow>level 2 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) < 0.75)
                    {
                        Debug.Log("<color=orange>level 3 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                    }
                    else if (Vector3.Distance(oldPos, newPosition) > 0.75)
                    {
                        Debug.Log("<color=red>level 4 jump </color>" + "Old: " + OldElement.ObjectName + "New: " +
                                  gridElement.ObjectName + " distance :  " + Vector3.Distance(oldPos, newPosition));
                        _inValid = true;
                        Debug.Log("is invalid retry...");
                    }
                }
                OldElement = gridElement;
                
                count--;
            }
            else
            {
                count=0;
            }
        }

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
        if (!_validGridRoutes.Any())
        {
            return;
        }
        else
        {
            Debug.Log("HELLO!");
        }
    }
    public void GenerateGridElementList(GameObject grid)
    {
        grid.gameObject.SetActive(true);
        
        Traverse(grid);

        if (_inValid)
        {
            Debug.Log("invalid");
        }
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
