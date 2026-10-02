using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Spline : MonoBehaviour
{
    [Serializable]
    public class SplinePoint
    {
        public enum SplineType
        {
            Quadratic,
            Cubic
        }

        public SplineType splineType = SplineType.Cubic;
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

            if (start == null || end == null || a == null) return Vector3.zero;

            if (splineType == SplineType.Quadratic)
            {
                var sa = Vector3.Lerp(start.position, a.position, t);
                var ae = Vector3.Lerp(a.position, end.position, t);

                return Vector3.Lerp(sa, ae, t);
            }
            else
            {
                if (b == null) return Vector3.zero;

                var sa = Vector3.Lerp(start.position, a.position, t);
                var ab = Vector3.Lerp(a.position, b.position, t);
                var be = Vector3.Lerp(b.position, end.position, t);

                var leftQuad = Vector3.Lerp(sa, ab, t);
                var rightQuad = Vector3.Lerp(ab, be, t);

                return Vector3.Lerp(leftQuad, rightQuad, t);
            }
        }

        public void GetPointGONonAlloc(ref GameObject[] arr)
        {
            arr[0] = start != null ? start.gameObject : null;
            arr[1] = a != null ? a.gameObject : null;
            if (splineType == SplineType.Quadratic)
            {
                arr[2] = end != null ? end.gameObject : null;
                arr[3] = null;
            }
            else
            {
                arr[2] = b != null ? b.gameObject : null;
                arr[3] = end != null ? end.gameObject : null;
            }
        }

        public void SetTransformsArray(GameObject[] arr)
        {
            start = arr.Length > 0 && arr[0] != null ? arr[0].transform : null;
            a = arr.Length > 1 && arr[1] != null ? arr[1].transform : null;
            if (splineType == SplineType.Quadratic)
            {
                end = arr.Length > 2 && arr[2] != null ? arr[2].transform : null;
                b = null;
            }
            else
            {
                b = arr.Length > 2 && arr[2] != null ? arr[2].transform : null;
                end = arr.Length > 3 && arr[3] != null ? arr[3].transform : null;
            }
        }
    }

    [SerializeField] int splineSamples = 7;
    [SerializeField] List<SplinePoint> splinePoints = new List<SplinePoint>();
    [SerializeField] Color startColor = Color.blue, endColor = Color.purple;

    [SerializeField] Transform pointParent;
    [SerializeField, HideInInspector] List<Transform> points = new List<Transform>();

    void OnValidate()
    {
        if (NeedsRegeneration()) RegeneratePoints();
    }

    int GetRequiredPointCount()
    {
        if (splinePoints == null || splinePoints.Count == 0) return 0;
        int count = 0;
        for (int i = 0; i < splinePoints.Count; i++)
        {
            if (splinePoints[i] == null) continue;
            int pointsForThis = (splinePoints[i].splineType == SplinePoint.SplineType.Quadratic) ? (i == 0 ? 3 : 2) : (i == 0 ? 4 : 3);
            count += pointsForThis;
        }
        return count;
    }

    bool NeedsRegeneration()
    {
        if (splinePoints == null) return points.Count != 0;
        if (points.Contains(null)) return true;
        if (points.Count != GetRequiredPointCount()) return true;

        int pointIndex = 0;
        for (int i = 0; i < splinePoints.Count; i++)
        {
            var sp = splinePoints[i];
            if (sp == null) continue;
            if (sp.splineType == SplinePoint.SplineType.Quadratic)
            {
                if (pointIndex + 2 >= points.Count) return true;
                if (sp.start != points[pointIndex] || sp.a != points[pointIndex + 1] || sp.end != points[pointIndex + 2]) return true;
                pointIndex += 2;
            }
            else
            {
                if (pointIndex + 3 >= points.Count) return true;
                if (sp.start != points[pointIndex] || sp.a != points[pointIndex + 1] || sp.b != points[pointIndex + 2] || sp.end != points[pointIndex + 3]) return true;
                pointIndex += 3;
            }
        }
        return false;
    }

    void RegeneratePoints()
    {
        if (pointParent == null) pointParent = transform;

        #if UNITY_EDITOR
        EditorApplication.delayCall -= UpdatePoints;
        EditorApplication.delayCall += UpdatePoints;
        #else
        UpdatePoints();
        #endif
    }

    void UpdatePoints()
    {
        if (this == null) return;
        if (pointParent == null) pointParent = transform;

        // remove dead points
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (points[i] == null)
            {
                points.RemoveAt(i);
            }
        }

        int requiredCount = GetRequiredPointCount();

        // remove excess
        if (points.Count > requiredCount)
        {
            for (int i = points.Count - 1; i >= requiredCount; i--)
            {
                if (points[i] != null)
                {
                    if (Application.isPlaying)
                        Destroy(points[i].gameObject);
                    else
                        DestroyImmediate(points[i].gameObject);
                }
                points.RemoveAt(i);
            }
        }
        
        // Spawn missing points
        else if (points.Count < requiredCount)
        {
            while (points.Count < requiredCount)
            {
                GameObject newGo = new GameObject($"Point_{points.Count}");
                newGo.transform.SetParent(pointParent);
                points.Add(newGo.transform);
            }
        }
        
        int pointIndex = 0;
        for (int i = 0; i < splinePoints.Count; i++)
        {
            var sp = splinePoints[i];
            if (sp == null) continue;

            if (sp.splineType == SplinePoint.SplineType.Quadratic)
            {
                Transform pStart = points[pointIndex];
                Transform pA = points[pointIndex + 1];
                Transform pEnd = points[pointIndex + 2];

                if (i == 0) pStart.name = $"Spline_{i} | Start Point";
                pA.name = $"Spline_{i} | Handle";
                pEnd.name = $"Spline_{i} | End Point";

                sp.AssignTransforms(pStart, pEnd, pA, null);
                pointIndex += 2;
            }
            else
            {
                Transform pStart = points[pointIndex];
                Transform pA = points[pointIndex + 1];
                Transform pB = points[pointIndex + 2];
                Transform pEnd = points[pointIndex + 3];

                if (i == 0) pStart.name = $"Spline_{i} | Start Point";
                pA.name = $"Spline_{i} | Start Handle";
                pB.name = $"Spline_{i} | End Handle";
                pEnd.name = $"Spline_{i} | End Point";

                sp.AssignTransforms(pStart, pEnd, pA, pB);
                pointIndex += 3;
            }
        }
    }

    void DestroySplineObjects(List<Transform> splinePoints)
    {
        for (int i = 0; i < splinePoints.Count; i++)
        {
            if (splinePoints[i] != null)
            {
                if (Application.isPlaying)
                    Destroy(splinePoints[i].gameObject);
                else
                    DestroyImmediate(splinePoints[i].gameObject);
            }
        }
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

            // draw handles
            if (splinePoints[i].splineType == SplinePoint.SplineType.Quadratic)
            {
                if (goBuffer[0] != null && goBuffer[1] != null)
                    Gizmos.DrawLine(goBuffer[0].transform.position, goBuffer[1].transform.position);
                if (goBuffer[2] != null && goBuffer[1] != null)
                    Gizmos.DrawLine(goBuffer[2].transform.position, goBuffer[1].transform.position);
            }
            else
            {
                if (goBuffer[0] != null && goBuffer[1] != null)
                    Gizmos.DrawLine(goBuffer[0].transform.position, goBuffer[1].transform.position);
                if (goBuffer[3] != null && goBuffer[2] != null)
                    Gizmos.DrawLine(goBuffer[3].transform.position, goBuffer[2].transform.position);
            }

            // repr spline
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
        if (splinePoints == null || index < 0 || index >= splinePoints.Count) return Vector3.zero;

        return splinePoints[index].EvaluatePoint(t);
    }

    public Vector3 EvaluateSplineGroup(float t)
    {
        if (splinePoints == null) return Vector3.zero;
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
    
    [ContextMenu("Reset Handles")]
    public void ResetHandles()
    {
        if (splinePoints == null) return;

        for (int i = 0; i < splinePoints.Count; i++)
        {
            var sp = splinePoints[i];
            if (sp == null) continue;

            if (sp.splineType == SplinePoint.SplineType.Quadratic)
            { 
                if (sp.a != null && sp.start != null)
                {
                    #if UNITY_EDITOR
                    Undo.RecordObject(sp.a, "Reset Handle");
                    #endif
                    sp.a.position = sp.start.position;
                }
            }
            else
            {
                if (sp.a != null && sp.start != null)
                {
                    #if UNITY_EDITOR
                    Undo.RecordObject(sp.a, "Reset Handle");
                    #endif
                    sp.a.position = sp.start.position;
                }

                if (sp.b != null && sp.end != null)
                {
                    #if UNITY_EDITOR
                    Undo.RecordObject(sp.b, "Reset Handle");
                    #endif
                    sp.b.position = sp.end.position;
                }
            }
        }
    }
}
