using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = System.Random;

public class RandomGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fixationPoint;
    private int _counter = 1;
    private List<RandomBlock> _Blocks;
    
    /*private void Start()
    {
        _Blocks = new List<RandomBlock>();

        _Blocks.Add(GenerateBlocks()); 
        
        /*for (int i = 0; i < 6; i++)
        {
            _Blocks.Add(GenerateBlocks());   
        }#1#
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetNextPosition();
        }
    }

    RandomBlock GenerateBlocks()
    {
        RandomBlock block = new RandomBlock();

        block.LargeGridClose = GetPositionList(24);
        /*block.LargeGridFar = GetPositionList(24);
        block.SmallGrid = GetPositionList(12);
        block.SmoothPursuit = GetPositionList(24);*/

        foreach (var item in block.LargeGridClose)
        {
            Debug.Log(item.Position);
        }
        
        // TODO fill out the rest

        return block;
    }

    private List<GridElement> GetPositionList(int numberOfElements)
    {
        List<GridElement> gridElements = new List<GridElement>();

        for (int i = 0; i < numberOfElements; i++)
        {
            // Debug.Log(i);
            GridElement grid = new GridElement {Position = GetNextPosition()};

            // TODO 
            /*grid.FixationDuration
            grid.MovementDuration*/
            if (grid.Position != Vector3.zero)
            {
                gridElements.Add(grid);
            }
        }
        
        return gridElements;
    }

    private Vector3 GetNextPosition()
    {
        List<RaycastHit> hitList = GetHitList();
        Random rand = new Random();
        int index = rand.Next(hitList.Count);
        
        if (hitList.Any())
        {
            _counter = 1;
            
            fixationPoint.transform.position = hitList[index].collider.transform.position;
            hitList[index].collider.gameObject.SetActive(false);
            return fixationPoint.transform.position;
        }

        if (_counter <= 5)
        {
            _counter++;
            GetNextPosition();
        }

        return default;
    }
    
    private List<RaycastHit> GetHitList()
    {
        RaycastHit[] hits;
        List<RaycastHit> hitList = new List<RaycastHit>();
        Vector3 halfExtents = new Vector3(.2f * _counter, .13f * _counter, .07f);
        // Debug.Log("halfExtents: " + halfExtents);

        hits = Physics.BoxCastAll(fixationPoint.transform.position, halfExtents, 
            fixationPoint.transform.forward, fixationPoint.transform.rotation, 300f);
        
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

    
}
