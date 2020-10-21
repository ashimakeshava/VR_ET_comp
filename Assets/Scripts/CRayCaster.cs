using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class CRayCaster : MonoBehaviour
{
    [SerializeField] private bool isRayCasting = false;
    [SerializeField] [Range(0, 10)] private float gizmosRayCastLength = 1;
    [SerializeField] private char separator = ',';
    [SerializeField] [Range(0, 10)] private float gizmosHelperRadius = 0.2f;
    private GameObject head;
    private GameObject eye;

   
    private Queue<string> unparsedBuffer;
    private Queue<List<string>> finalBuffer;
    private bool readFinished = false;
    

    private string csvHeaders = "";
    public string _fileName;

    public string FileName
    {
        get => _fileName;
        set => _fileName = value;
    }


    private Vector3 _gizmosEyePos;
    private Vector3 _gizmosEyeDir;
    private Vector3 _gizmosNosePos;
    private Vector3 _gizmosNoseDir;

    void Start()
    {
        

        _gizmosEyePos = new Vector3();
        _gizmosEyeDir = Vector3.forward;
        _gizmosNosePos = new Vector3();
        _gizmosNoseDir = Vector3.forward;
        head = new GameObject();
        eye = new GameObject();
        eye.transform.parent = head.transform;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        StopRayCaster();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRayCaster();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            StopRayCaster();
        }
    }

    private void StopRayCaster()
    {
        isRayCasting = false;
        StopAllCoroutines();
    }

    private async void StartRayCaster()
    {
        isRayCasting = true;
        StartCoroutine(ReadCSV(_fileName));
        StartCoroutine(ParseCSVCombined());
        StartCoroutine(WriteCSV(_fileName+"~"));
            
    }

    public IEnumerator ReadCSV(string fileName)
    {
        readFinished = false;
        unparsedBuffer = new Queue<string>();
        bool isFirstLine = true;
        using (StreamReader csv = new StreamReader(fileName))
        {
            var csvLine = "";
            while ((csvLine = csv.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    csvHeaders = csvLine;
                    isFirstLine = false;
                }
                else
                    unparsedBuffer.Enqueue(csvLine);

                Debug.LogFormat("unparsed buffer size = {0}", unparsedBuffer.Count);
                yield return null;
            }

            readFinished = true;
        }
    }

    public IEnumerator ParseCSVTransform()
    {
        finalBuffer = new Queue<List<string>>();

        while (true)
        {
            if (unparsedBuffer.Count != 0)
            {
                List<string> parsedLine = unparsedBuffer.Dequeue().Split(separator).ToList();
                //Debug.LogError(parsedLine.Count);
                Tuple<Vector3, Vector3> hitResult = TransformCoord(parsedLine);
                Tuple<Vector3, Vector3> trans = TransformCoord(parsedLine);
                //Transfomed eye pos
                parsedLine[63] = hitResult.Item1.x.ToString();
                parsedLine[64] = hitResult.Item1.y.ToString();
                parsedLine[65] = hitResult.Item1.z.ToString();
                // Transformed eye dir
                parsedLine[66] = hitResult.Item2.x.ToString();
                parsedLine[67] = hitResult.Item2.y.ToString();
                parsedLine[68] = hitResult.Item2.z.ToString();
               
                finalBuffer.Enqueue(parsedLine);
                Debug.LogFormat("final buffer size = {0}", finalBuffer.Count);
            }
            else
            {
                Debug.LogError("unparsed buffer empty");
            }

            yield return null;
        }
    }
    public IEnumerator ParseCSVCombined()
    {
        finalBuffer = new Queue<List<string>>();

        while (true)
        {
            if (unparsedBuffer.Count != 0)
            {
                List<string> parsedLine = unparsedBuffer.Dequeue().Split(separator).ToList();
                //Debug.LogError(parsedLine.Count);
                Tuple<Vector3, Vector3> transformResult = TransformCoord(parsedLine);
                
                //Transfomed eye pos
                parsedLine[61] = transformResult.Item1.x.ToString();
                parsedLine[62] = transformResult.Item1.y.ToString();
                parsedLine[63] = transformResult.Item1.z.ToString();
                // Transformed eye dir
                parsedLine[64] = transformResult.Item2.x.ToString();
                parsedLine[65] = transformResult.Item2.y.ToString();
                parsedLine[66] = transformResult.Item2.z.ToString();
               
                Tuple<string, string> hitResult = CastRay(parsedLine);
                // eye hit
                //parsedLine[79] = hitResult.Item1;
                // nose hit
                parsedLine[67] = hitResult.Item2;
                finalBuffer.Enqueue(parsedLine);
                Debug.LogFormat("final buffer size = {0}", finalBuffer.Count);
            }
            else
            {
                Debug.LogError("unparsed buffer empty");
            }

            yield return null;
        }
    }
    public IEnumerator ParseCSV()
    {
        finalBuffer = new Queue<List<string>>();

        while (true)
        {
            if (unparsedBuffer.Count != 0)
            {
                List<string> parsedLine = unparsedBuffer.Dequeue().Split(separator).ToList();

                Tuple<string, string> hitResult = CastRay(parsedLine);
                // eye hit
                parsedLine[79] = hitResult.Item1;
                // nose hit
                parsedLine[80] = hitResult.Item2;
               
                finalBuffer.Enqueue(parsedLine);
                Debug.LogFormat("final buffer size = {0}", finalBuffer.Count);
            }
            else
            {
                Debug.LogError("unparsed buffer empty");
            }

            yield return null;
        }
    }

    public IEnumerator WriteCSV(string fileName)
    {
        using (StreamWriter csv = new StreamWriter(fileName))
        {
            while (csvHeaders == "")
            {
                yield return null;
            }

            csv.WriteLine(csvHeaders);
            while (true)
            {
                if (finalBuffer.Count != 0)
                {
                    string lineToWrite = "";
                    foreach (string section in finalBuffer.Dequeue())
                    {
                        lineToWrite += (section + separator);
                    }
                    //Debug.LogError(lineToWrite);
                    csv.WriteLine(lineToWrite);
                }

                if (readFinished && unparsedBuffer.Count == 0 && finalBuffer.Count == 0)
                {
                    StopAllCoroutines();
                }

                yield return null;
            }
        }
    }

    private Tuple<Vector3, Vector3> TransformCoord(List<string> line)
    {
       
        try
        {
            Vector3 noseOrigin = head.transform.position = new Vector3(float.Parse(line[31]), float.Parse(line[32]), float.Parse(line[33]));
            Vector3 noseDirection = new Vector3(float.Parse(line[34]), float.Parse(line[35]), float.Parse(line[36]));
            head.transform.forward = noseDirection;
            eye.transform.localPosition = new Vector3(float.Parse(line[21]), float.Parse(line[22]), float.Parse(line[23]));
            //eye.transform.forward = new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27]));
            Vector3 eyeOrigin = eye.transform.position;
            
            //Vector3 eyeDirection = eye.transform.TransformDirection(new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27])));
            
            //Vector3 eyeDirection = new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27])) * head.transform.rotation;
            Vector3 eyeDirection = head.transform.TransformDirection(new Vector3(float.Parse(line[24]), float.Parse(line[25]), float.Parse(line[26])));
            _gizmosEyePos = eyeOrigin;
            _gizmosEyeDir = eyeDirection;
            _gizmosNosePos = noseOrigin;
            _gizmosNoseDir = noseDirection;
            
            Debug.LogWarningFormat(
                "Converting with eye origin ({0}), eye direction ({1}), nose origin ({2}) and nose direction ({3})",
                eyeOrigin, eyeDirection, noseOrigin, noseDirection);
            

            return new Tuple<Vector3, Vector3>(eyeOrigin, eyeDirection);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("ParseError at {0} - {1} - {2} - {3}" ,line[0],line[2],line[3],line[4]);
            Vector3 errorVec = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            return new Tuple<Vector3, Vector3>(errorVec, errorVec);
        }
       
    }
    private Tuple<string, string> CastRay(List<string> line)
    {
        
        string eyeHit = "";
        string noseHit = "";
        try
        {
            Vector3 noseOrigin = head.transform.position = new Vector3(float.Parse(line[31]), float.Parse(line[32]), float.Parse(line[33]));
            Vector3 noseDirection = new Vector3(float.Parse(line[34]), float.Parse(line[35]), float.Parse(line[36]));
            head.transform.forward = noseDirection;
            eye.transform.localPosition = new Vector3(float.Parse(line[21]), float.Parse(line[22]), float.Parse(line[23]));
            //eye.transform.forward = new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27]));
            Vector3 eyeOrigin = eye.transform.position;
            
            //Vector3 eyeDirection = eye.transform.TransformDirection(new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27])));
            
            //Vector3 eyeDirection = new Vector3(float.Parse(line[25]), float.Parse(line[26]), float.Parse(line[27])) * head.transform.rotation;
            Vector3 eyeDirection = head.transform.TransformDirection(new Vector3(float.Parse(line[24]), float.Parse(line[25]), float.Parse(line[26])));
            _gizmosEyePos = eyeOrigin;
            _gizmosEyeDir = eyeDirection;
            _gizmosNosePos = noseOrigin;
            _gizmosNoseDir = noseDirection;
            RaycastHit eyeRayHit = new RaycastHit();
            //RaycastHit noseRayHit = new RaycastHit();
            Debug.LogWarningFormat(
                "raycasting with eye origin ({0}), eye direction ({1}), nose origin ({2}) and nose direction ({3})",
                eyeOrigin, eyeDirection, noseOrigin, noseDirection);
            if (Physics.Raycast(eyeOrigin, eyeDirection, out eyeRayHit, Mathf.Infinity))
            {
                eyeHit = eyeRayHit.collider.name;
            }

            /*if (Physics.Raycast(noseOrigin, noseDirection, out noseRayHit, Mathf.Infinity))
            {
                noseHit = noseRayHit.collider.name;
            }*/

            return new Tuple<string, string>(eyeHit, eyeHit);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("ParseError at {0} - {1} - {2} - {3}" ,line[0],line[2],line[3],line[4]);
            return new Tuple<string, string>("ERROR", "ERROR");
        }
       
    }

    private void OnDrawGizmos()
    {
        if (isRayCasting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_gizmosNosePos, gizmosHelperRadius);
            Gizmos.DrawRay(_gizmosNosePos, _gizmosNoseDir * gizmosRayCastLength);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_gizmosEyePos, gizmosHelperRadius);
            Gizmos.DrawRay(_gizmosEyePos, _gizmosEyeDir * gizmosRayCastLength);
        }
    }
}

[CustomEditor(typeof(CRayCaster))]
public class CRayCasterInspector : Editor
{
    private CancellationToken token;
    private CancellationToken casterToken;
    private CancellationToken writerToken;

    public override void OnInspectorGUI()
    {
        CRayCaster raycaster = (CRayCaster) target;
        base.OnInspectorGUI();
        if (GUILayout.Button("select csv file"))
            raycaster.FileName = EditorUtility.OpenFilePanel("CSV data file", Application.dataPath, "csv");
    }
}