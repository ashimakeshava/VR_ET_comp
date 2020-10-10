using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = System.Random;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager Instance { get ; private set; } 
    
    [SerializeField] private GameObject fixationPoint;
    [SerializeField] private GameObject largeGrid1;
    [SerializeField] private GameObject largeGrid2;
    [SerializeField] private GameObject smallGrid;
    [SerializeField] private GameObject smoothPursuit;
    [SerializeField] private GameObject mainCamera;
    
    private List<Block> _blocks;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    

    private void Start()
    {
        _blocks = new List<Block>();

        for (int i = 0; i < 6; i++)
        {
            _blocks.Add(GetComponent<BlockGenerator>().GenerateBlock());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GetCurrentActiveGrid();
            //GetComponent<RouteGenerator>().GenerateGridRoute(GetCurrentActiveGrid());
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < smallGrid.transform.childCount;i++)
            {
                smallGrid.transform.GetChild(i).gameObject.SetActive(true);

            }
            fixationPoint.transform.position= new Vector3(0,0,1);
            Debug.Log("___________________________________-----_____________________________");
        }
    }

    
    
    public GameObject GetFixationPoint()
    {
        return fixationPoint;
    }

    public GameObject GetCurrentActiveGrid()
    {
        var GridList = GetGridList();
        GameObject activeGrid=new GameObject();
        bool found=false;
        foreach (var grid in GridList)
        {
            if (grid.activeInHierarchy&& !found)
            {
                activeGrid = grid;
                found = true;
            }

            if (grid.activeInHierarchy && found)
            {
                Debug.LogWarning("Two Grids are active, this can cause problems, deactivate one");
            }
        }
        return activeGrid;
    }

    private List<GameObject> GetGridList()
    {
        List<GameObject> GridList = new List<GameObject>();
        foreach (Transform child in mainCamera.transform)
        {
            if (child.gameObject != fixationPoint)
                GridList.Add(child.gameObject);
        }

        return GridList;
    }

    public GameObject GetLargeGrid1()
    {
        return largeGrid1;
    }
    
    public GameObject GetLargeGrid2()
    {
        return largeGrid2;
    }
    public GameObject GetSmallGrid()
    {
        return smallGrid;
    }
    
    public GameObject GetSmoothPursuit()
    {
        return smoothPursuit;
    }
}

        
    // TODO implement movement
    // TODO smoothPursuit has too few elements