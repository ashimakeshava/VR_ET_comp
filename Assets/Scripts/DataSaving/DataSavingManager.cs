using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataSavingManager : MonoBehaviour
{
    private RouteGenerator routeGenerator;

    private List<GridElement> _gridRoute;

    [SerializeField] private String SavePath;
    private void Start()
    {
        routeGenerator= GetComponent<RouteGenerator>();
        SavePath = Application.persistentDataPath;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StoreGridRoute(routeGenerator.GetGridRoute());
            
            SaveToJson(_gridRoute,"test.txt");
        }
    }

    private void StoreGridRoute(List<GridElement> gridRoute)
    {
        _gridRoute = gridRoute;
    }


    private List <string> ConvertToJson<T>(List<T> genericList)
    {
        List<string> list = new List<string>();
        list.Add("[");
        foreach (var g in genericList)
        {
           // Debug.Log(g.ToString());
            string jsonString = JsonUtility.ToJson(g, true);
            list.Add(jsonString);
        }
        
        list.Add("]");

        return list;
    }


    public void SaveToJson<T>(List<T> file, string  fileName)
    {
        var RouteData = ConvertToJson(file);

        string path = GetPathForSaveFile(fileName);
        
        // I implemented the LoopAR Data saving, this time I got Access Violation.  I dont get why,  I needed a new File Stream Implementation 
        FileStream fileStream= new FileStream(path, FileMode.Create);
        using (var fileWriter= new StreamWriter(fileStream))
        {
            foreach (var line in RouteData)
            {
                fileWriter.WriteLine(line);
            }
        }
        
        
        Debug.Log("saved  " +fileName + " to : " + SavePath );
    }
    
    
    
    private string GetPathForSaveFile(string fileName)
    {
        return Path.Combine(SavePath, fileName);
    }
}
