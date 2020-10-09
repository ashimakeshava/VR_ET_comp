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

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<GridElementsGenerator>().Traverse(smallGrid);
        }
    }*/
    
    
    public GameObject GetFixationPoint()
    {
        return fixationPoint;
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