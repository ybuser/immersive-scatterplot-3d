using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAnimation : MonoBehaviour
{
    public List<Vector3> positionsList = new List<Vector3>();
    private int currentPositionIndex = 0;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;

    void Start()
    {
        if (positionsList.Count > 1)
        {
            startPosition = positionsList[0];
            endPosition = positionsList[1];
        }
        else if (positionsList.Count == 1)
        {
            transform.position = positionsList[0]; // If only one position, set it and end.
            this.enabled = false; // Disable the script to stop Update calls.
        }
        else
        {
            this.enabled = false; // Disable the script if no positions.
        }
    }

    void Update()
    {
        if (t < 1)
        {
            t += Time.deltaTime; // Increase t by the delta time.
            Debug.Log($"Animating Point: {name}, Start: {startPosition}, End: {endPosition}, t: {t}");
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex < positionsList.Count - 1)
            {
                t = 0;
                startPosition = positionsList[currentPositionIndex];
                endPosition = positionsList[currentPositionIndex + 1];
            }
        }
    }
}
