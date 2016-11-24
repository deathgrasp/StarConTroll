using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineScript {

	// Use this for initialization
	void Start () {
        var points=new Vector3[3];
	        points[0]=new Vector3(0,2,0);
        points[1] = new Vector3(2, -2, 0);
        points[2] = new Vector3(4, 2, 0);
        //points[3] = new Vector3(6, -2, 0);
        //points[4] = new Vector3(8, 2, 0);

       
    }
	
	// Update is called once per frame
    private void Update()
    {
    }

    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
     public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Count;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints);
    }

    public LineRenderer LineRenderer;
 
}
