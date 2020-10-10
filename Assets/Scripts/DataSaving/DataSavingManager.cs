using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class DataSavingManager : MonoBehaviour
{
    private RouteGenerator routeGenerator;

    private List<GridElement> _gridRoute;

    [SerializeField] private String SavePath;
    private void Start()
    {
        routeGenerator= GetComponent<RouteGenerator>();
        _gridRoute= new List<GridElement>();
        SavePath = Application.persistentDataPath;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StoreGridRoute(routeGenerator.GetGridRoute());
            
            SaveList(_gridRoute,"test.txt");
        }
        
        
        if(Input.GetKeyDown(KeyCode.A))
        {
            
            string filePath = GetPathForSaveFile("test.txt");
            Debug.Log(filePath);
            _gridRoute = LoadList<GridElement>(filePath);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            routeGenerator.CreateTestFile();
            TestFrame bunny = routeGenerator.bunny;
            Save(bunny,"bunny.txt");
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            
            string filePath = GetPathForSaveFile("test.txt");
            Debug.Log(filePath);
            _gridRoute = LoadList<GridElement>(filePath);
        }
    }

    private void StoreGridRoute(List<GridElement> gridRoute)
    {
        _gridRoute = gridRoute;
    }


    private List <string> ConvertToJson<T>(List<T> genericList)
    {
        List<string> list = new List<string>();
        //list.Add("[");
        foreach (var g in genericList)
        {
           // Debug.Log(g.ToString());
            string jsonString = JsonUtility.ToJson(g);
            list.Add(jsonString);
        }
        
        //list.Add("]");

        return list;
    }
    
    private string ConvertToJson<T>(T generic)
    {
        string json= JsonUtility.ToJson(generic);
    
        return json;
    }


  
    
    
    

    public List<T> LoadList<T>(string path)
    {
        
        List<T> genericList=new List<T>();

        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            foreach (var line in data)
            {
                T tmp= JsonUtility.FromJson<T>(line);
                genericList.Add(tmp);
            }
            
        }
        
        return genericList;
        
    }
    
    public T Load<T>(string path)
    {
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(data);
        }
        else
        {
            throw new Exception("file not found");
        }
    }
    
    public void Save<T>(T file, string  fileName)
    {
        var data = ConvertToJson(file);

        string path = GetPathForSaveFile(fileName);
        
        FileStream fileStream= new FileStream(path, FileMode.Create);
        using (var fileWriter= new StreamWriter(fileStream))
        {
            fileWriter.WriteLine(data);
        }
        
        
        Debug.Log("saved  " +fileName + " to : " + SavePath );
    }
    
    public void SaveList<T>(List<T> file, string  fileName)
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
