using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataPlotter : MonoBehaviour
{
    public string inputfile;
    private List<Dictionary<string, object>> pointList; // List for holding data from CSV reader
    private Dictionary<string, Color> stateToColorMap = new Dictionary<string, Color>(); // Dictionary to map "State"s to colors

    
    public int columnX = 7;
    public int columnY = 8;
    public int columnZ = 9;
    public string xName;
    public string yName;
    public string zName;
    public float plotScale = 10;

    public GameObject PointPrefab;
    public GameObject PointHolder;
    public GameObject AxisPrefab;
    public GameObject LabelPrefab;
//
    private Dictionary<string, GameObject> nameToPointMap = new Dictionary<string, GameObject>();

//

    void Start () {
        pointList = CSVReader.Read(inputfile);
        Debug.Log(pointList);
        // Declare list of strings, fill with keys (column names)
        List<string> columnList = new List<string>(pointList[1].Keys);
        Debug.Log("There are " + columnList.Count + " columns in CSV");
        foreach (string key in columnList)
            Debug.Log("Column name is " + key);

        xName = columnList[columnX];
        yName = columnList[columnY];
        zName = columnList[columnZ];

        float xMax = FindMaxValue(xName);
        float yMax = FindMaxValue(yName);
        float zMax = FindMaxValue(zName);
        float xMin = FindMinValue(xName);
        float yMin = FindMinValue(yName);
        float zMin = FindMinValue(zName);

        HashSet<string> uniqueStates = new HashSet<string>();
        foreach (var point in pointList) {
            uniqueStates.Add(point["State"].ToString());
        }

        float hueIncrement = 1.0f / uniqueStates.Count;
        float currentHue = 0;
        foreach (string state in uniqueStates) {
            stateToColorMap[state] = Color.HSVToRGB(currentHue, 1, 1);
            currentHue += hueIncrement;
        }


        for (var i = 0; i < pointList.Count; i++) {
            // normalized
            float x = NormalizeValue(System.Convert.ToSingle(pointList[i][xName]), xMin, xMax);
            float y = NormalizeValue(System.Convert.ToSingle(pointList[i][yName]), yMin, yMax);
            float z = NormalizeValue(System.Convert.ToSingle(pointList[i][zName]), zMin, zMax);

            // Use "Site Num" as unique identifier
            string dataName = pointList[i]["Site Num"].ToString();

//

            if (!nameToPointMap.ContainsKey(dataName)) {
                GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
                dataPoint.transform.parent = PointHolder.transform;
                string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName];
                dataPoint.transform.name = dataPointName;
                string state = pointList[i]["State"].ToString();
                if (stateToColorMap.ContainsKey(state)) {
                    dataPoint.GetComponent<Renderer>().material.color = stateToColorMap[state];
                } else {
                    dataPoint.GetComponent<Renderer>().material.color = Color.black; // Default color if state is not in the dictionary for some reason
                }

                nameToPointMap[dataName] = dataPoint;
            }
            else {
                GameObject dataPoint = nameToPointMap[dataName];
                PointAnimation pointAnim = dataPoint.GetComponent<PointAnimation>();
                if (pointAnim == null)
                {
                    pointAnim = dataPoint.AddComponent<PointAnimation>();
                }
                // pointAnim.Positions.Add(new Vector3(x, y, z) * plotScale);
                pointAnim.positionsList.Add(new Vector3(x, y, z) * plotScale); // Assuming PointAnimation has a property called "positionsList"
            }
//
        }
        CreateAxisAndLabels();
    }

    private float FindMaxValue(string columnName) {
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        for (var i = 1; i < pointList.Count; i++) {
            try
            {
                if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                    maxValue = Convert.ToSingle(pointList[i][columnName]);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Error parsing {pointList[i][columnName]} from column {columnName}");
                throw e;
            }
        }
        return maxValue;
    }

    private float FindMinValue(string columnName) {
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        for (var i = 1; i < pointList.Count; i++) {
            try
            {
                if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                    minValue = Convert.ToSingle(pointList[i][columnName]);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Error parsing {pointList[i][columnName]} from column {columnName}");
                throw e;
            }
        }
        return minValue;
    }

    private float NormalizeValue(float value, float minValue, float maxValue) {
        if (maxValue - minValue == 0) return 0.5f; // Return a default value, since it's a constant value
        return (value - minValue) / (maxValue - minValue);
    }

    void CreateAxisAndLabels()
    {
        float axisLength = plotScale + 2; // or any value you find suitable

        // X Axis
        GameObject xAxis = Instantiate(AxisPrefab, new Vector3(axisLength / 2, 0, 0), Quaternion.identity);
        xAxis.transform.localScale = new Vector3(axisLength, 0.1f, 0.1f);
        CreateLabel(new Vector3(axisLength, 0, 0), xName);

        // Y Axis
        GameObject yAxis = Instantiate(AxisPrefab, new Vector3(0, axisLength / 2, 0), Quaternion.identity);
        yAxis.transform.localScale = new Vector3(0.1f, axisLength, 0.1f);
        CreateLabel(new Vector3(0, axisLength, 0), yName);

        // Z Axis
        GameObject zAxis = Instantiate(AxisPrefab, new Vector3(0, 0, axisLength / 2), Quaternion.identity);
        zAxis.transform.localScale = new Vector3(0.1f, 0.1f, axisLength);
        CreateLabel(new Vector3(0, 0, axisLength), zName);
    }

    void CreateLabel(Vector3 position, string labelText)
    {
        GameObject label = Instantiate(LabelPrefab, position, Quaternion.identity);
        TextMesh tMesh = label.GetComponent<TextMesh>();
        if (tMesh != null)
        {
            tMesh.text = labelText;
        }
    }
}
