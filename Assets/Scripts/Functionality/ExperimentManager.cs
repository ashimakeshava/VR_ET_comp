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
    
    
    private List<Block> _blocks;
    private List<GridElement> _gridElements;


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
        _gridElements = new List<GridElement>();

        _blocks.Add(GenerateBlocks());
        
        /*for (int i = 0; i < 6; i++)
        {
            _blocks.Add(GenerateBlocks());   
        }*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log(GetNextPosition());
        }
    }

    
    
    Block GenerateBlocks()
    {
        Block block = new Block();
        
        block.LargeGridClose = GetPositionList(25);
        /*block.LargeGridFar = GetPositionList(24);
        block.SmallGrid = GetPositionList(12);
        block.SmoothPursuit = GetPositionList(24);*/

        // TODO fill out the rest

        return block;
    }

    private List<GridElement> GetPositionList(int numberOfElements)
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            Debug.Log("i: " + i);
            GridElement grid = new GridElement();
            
            // GetNextPosition(grid);

            // TODO 
            /*grid.FixationDuration
            grid.MovementDuration*/
            
            /*if (grid.Position != Vector3.zero)
            {
                Debug.Log("pos: " + grid.Position);
                gridElements.Add(grid);
            }*/
            /*gridElements.Add(grid);
            Debug.Log("pos: " + grid.Position);*/

        }

        Debug.Log("Grid element count: " + _gridElements.Count);
        return _gridElements;
    }

    /*private void GetNextPosition(GridElement grid, int counter=1)
    {
        List<RaycastHit> hitList = GetHitList(.2f*counter, .13f*counter);
        Random rand = new Random();
        int index = rand.Next(hitList.Count);
        
        if (hitList.Any())
        {
            counter = 1;
            
            fixationPoint.transform.position = hitList[index].collider.transform.position;
            grid.Position = hitList[index].collider.transform.position;
            Debug.Log("name: " + hitList[index].collider.name);
            
            hitList[index].collider.gameObject.SetActive(false);
        }

        if (counter <= 5)
        {
            counter += 1;
            GetNextPosition(counter);
        }
    }*/

    public GameObject GetFixationPoint()
    {
        return fixationPoint;
    }
}
