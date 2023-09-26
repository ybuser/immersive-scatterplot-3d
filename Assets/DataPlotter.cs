using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataPlotter : MonoBehaviour
{
    public string inputfile;
    private List<Dictionary<string, object>> pointList; // List for holding data from CSV reader
    
    public int columnX = 0;
    public int columnY = 1;
    public int columnZ = 2;
    public string xName;
    public string yName;
    public string zName;
    public float plotScale = 10;

    public GameObject PointPrefab;
    public GameObject PointHolder;
    
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

        for (var i = 0; i < pointList.Count; i++) {
            // normalized
            float x = (System.Convert.ToSingle(pointList[i][xName]) - xMin) / (xMax - xMin);
            float y = (System.Convert.ToSingle(pointList[i][yName]) - yMin) / (yMax - yMin);
            float z = (System.Convert.ToSingle(pointList[i][zName]) - zMin) / (zMax - zMin);
            
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z)* plotScale, Quaternion.identity);
            dataPoint.transform.parent = PointHolder.transform;
            string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName];
            dataPoint.transform.name = dataPointName;
            dataPoint.GetComponent<Renderer>().material.color = new Color(x,y,z, 1.0f);
        }
        // GameObject.Find("5.1 3.5 1.4").GetComponent<TextMesh>().text = xName;
    }

    private float FindMaxValue(string columnName) {
        float maxValue = Convert.ToSingle(pointList[0][columnName]);
        for (var i = 0; i < pointList.Count; i++) {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }
        return maxValue;
    }

    private float FindMinValue(string columnName) {
        float minValue = Convert.ToSingle(pointList[0][columnName]);
        for (var i = 0; i < pointList.Count; i++) {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }
       return minValue;
   }

}
