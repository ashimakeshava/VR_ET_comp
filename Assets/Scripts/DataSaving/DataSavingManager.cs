using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class DataSavingManager : MonoBehaviour
{
    public static DataSavingManager Instance { get ; private set; } 
    private RouteGenerator routeGenerator;

    private List<GridElement> _gridRoute;
    [SerializeField] private String SavePath;

    private string _participantId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        routeGenerator= GetComponent<RouteGenerator>();
        _gridRoute= new List<GridElement>();
        SavePath = Application.persistentDataPath;
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


  
    
    
    

    public List<T> LoadFileList<T>(string FileName)
    {
        string path = GetPathForSaveFile(FileName);
        List<T> genericList=new List<T>();

        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            foreach (var line in data)
            {
                T tmp= JsonUtility.FromJson<T>(line);
                genericList.Add(tmp);
            }
            return genericList;
        }
        else
        {
            throw new Exception("file not found " + path);
        }
    }
    
    public T LoadFile<T>(string DataName)
    {
        string path = GetPathForSaveFile(DataName);
        if (File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            T tmp= JsonUtility.FromJson<T>(data[0]);
            return tmp;
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
        var stringList = ConvertToJson(file);

        string path = GetPathForSaveFile(fileName);
        
        // I implemented the LoopAR Data saving, this time I got Access Violation.  I dont get why,  I needed a new File Stream Implementation 
        FileStream fileStream= new FileStream(path, FileMode.Create);
        using (var fileWriter= new StreamWriter(fileStream))
        {
            foreach (var line in stringList)
            {
                fileWriter.WriteLine(line);
            }
        }
        
        
        Debug.Log("saved  " +fileName + " to : " + SavePath );
    }
    
    private string GetPathForSaveFile(string fileName, string format=".json")
    {
        string name = fileName + format;
        // return Path.Combine(Application.persistentDataPath, name);
        return Path.Combine(Path.Combine(Application.persistentDataPath, "Sub_" + _participantId), name);
    }

    public void SetParticipantID(string id)
    {
        _participantId = id;
    }
}
