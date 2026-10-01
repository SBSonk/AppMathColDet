using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [Serializable]
    public class SplinePoint
    {
        public Transform start, end, a, b;

        public void AssignTransforms(Transform start, Transform end, Transform a, Transform b)
        {
            this.start = start;
            this.end = end;
            this.a = a;
            this.b = b;
        }

        // ref: https://www.summbit.com/blog/bezier-curve-guide/
        public Vector3 EvaluatePoint(float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 point;

            //temp
            var sa = Vector3.Lerp(start.position, a.position, t);
            var ab = Vector3.Lerp(a.position, b.position, t);
            var be = Vector3.Lerp(b.position, end.position, t);

            var leftQuad = Vector3.Lerp(sa, ab, t);
            var rightQuad = Vector3.Lerp(ab, be, t);
            
            point = Vector3.Lerp(leftQuad, rightQuad, t);

            return point;
        }

        public void GetPointGONonAlloc(ref GameObject[] arr)
        {
            arr[0] = start.gameObject;
            arr[1] = a.gameObject;
            arr[2] = b.gameObject;
            arr[3] = end.gameObject;
        }

        public void SetTransformsArray(GameObject[] arr)
        {
            start = arr[0].transform;
            a = arr[1].transform;
            b = arr[2].transform;
            end = arr[3].transform;
        }
    }

    [SerializeField] int splineSamples = 7;
    [SerializeField] List<SplinePoint> splinePoints;
    [SerializeField] Color startColor = Color.blue, endColor = Color.purple;

    [SerializeField] Transform pointParent;
    [SerializeField, HideInInspector] List<Transform> points = new List<Transform>();

    void OnValidate()
    {
        if (points.Count != (splinePoints.Count * 4 - (splinePoints.Count-1))) RegeneratePoints();
    }

    void RegeneratePoints()
    {
        if (pointParent == null) pointParent = transform;
        
        EditorApplication.delayCall += () =>
        {
            if (points.Count > 0) // delete points if alr exist
            {
                DestroySplineObjects(points);
            }

            // for each spline create 4 points
            string[] names = {"a", "b", "c", "d"};
            GameObject prevLast = null;
            GameObject[] gameObjects = new GameObject[4];
            for (int i = 0; i < splinePoints.Count; i++)
            {
                if (prevLast == null) // make 4 points initially
                {
                    for (int g = 0; g < 4; g++) 
                    {
                        gameObjects[g] = new GameObject(name: $"Spline_{i} | {names[g]}");

                        gameObjects[g].transform.SetParent(pointParent);

                        points.Add(gameObjects[g].transform);
                    }
                } 
                else // use prev 4th as first = make 3 points
                {
                    gameObjects[0] = prevLast;
                    for (int g = 1; g < 4; g++) 
                    {
                        gameObjects[g] = new GameObject(name: $"Spline_{i} | {names[g]}");

                        gameObjects[g].transform.SetParent(pointParent);

                        points.Add(gameObjects[g].transform);
                    }
                }

                prevLast = gameObjects[3];

                // assign to spline point
                splinePoints[i].SetTransformsArray(gameObjects);
            }
        };
        
    }

    void DestroySplineObjects(List<Transform> splinePoints)
    {
        for (int i = 0; i < splinePoints.Count; i++) DestroyImmediate(splinePoints[i].gameObject);
        splinePoints.Clear();
    }

    void OnDrawGizmos()
    {
        if (points.Count == 0 || splinePoints.Count == 0) return;

        float steps = 1f / splinePoints.Count;
        GameObject[] goBuffer = new GameObject[4];
        for (int i = 0; i < splinePoints.Count; i++)
        {
            Gizmos.color = Color.Lerp(startColor, endColor, steps * i);

            splinePoints[i].GetPointGONonAlloc(ref goBuffer);

            // draw handles, dang thats cool
            Gizmos.DrawLine(goBuffer[0].transform.position, goBuffer[1].transform.position);
            Gizmos.DrawLine(goBuffer[3].transform.position, goBuffer[2].transform.position);

            // repr spline shit
            float interval = 1f / splineSamples;
            for (int seg = 0; seg < splineSamples-1; seg++)
            {
                Vector3 currPoint = splinePoints[i].EvaluatePoint(interval * seg);
                Vector3 nextPoint = splinePoints[i].EvaluatePoint(interval * (seg + 1));

                Gizmos.DrawLine(currPoint, nextPoint);
            }
        }
    }

    public Vector3 EvaluateSpline(int index, float t)
    {
        if (index < 0 || index >= splinePoints.Count) return Vector3.zero; // im running way too late for prop error handling

        return splinePoints[index].EvaluatePoint(t);
    }

    public Vector3 EvaluateSplineGroup(float t)
    {
        int splineCount = splinePoints.Count;
        if (splineCount == 0) return Vector3.zero;

        float scaledT = t * splineCount;
        int index = Mathf.FloorToInt(scaledT);

        if (index >= splineCount)
        {
            index = splineCount-1;
            scaledT = splineCount;
        }

        float adjT = scaledT - index;

        return EvaluateSpline(index, adjT);
    }
}
