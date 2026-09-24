using UnityEngine;

public static class CollisionHelpers
{
    public static bool IsPointInConeXZ(Vector3 origin, Vector3 forward, Vector3 target, float range, float coneAngleDegrees, float targetRadius = 0f)
    {
        Vector3 diff = target - origin;
        diff.y = 0f;
        forward.y = 0f;

        float sqrDistance = Vector3.Dot(diff, diff);
        float maxRange = range + targetRadius;
        if (sqrDistance > maxRange * maxRange) return false;

        float distance = Mathf.Sqrt(sqrDistance);
        if (distance <= Mathf.Max(targetRadius, 0.001f)) return true;

        Vector3 normalizedForward = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;
        Vector3 dirToTarget = diff / distance;

        float dot = Vector3.Dot(normalizedForward, dirToTarget);

        float halfAngleRad = (coneAngleDegrees * 0.5f) * Mathf.Deg2Rad;
        float angularAllowance = targetRadius > 0f && distance > targetRadius ? Mathf.Asin(Mathf.Clamp01(targetRadius / distance)) : 0f;

        float effectiveThreshold = Mathf.Cos(Mathf.Min(halfAngleRad + angularAllowance, Mathf.PI));

        return dot >= effectiveThreshold;
    }

    public static bool IsPointInLineSegmentXZ(Vector3 origin, Vector3 forward, Vector3 target, float lineLength, float lineWidth, float targetRadius = 0f)
    {
        Vector3 diff = target - origin;
        diff.y = 0f;
        forward.y = 0f;

        Vector3 normalizedForward = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;

        float projection = Vector3.Dot(diff, normalizedForward);

        if (projection < -targetRadius || projection > lineLength + targetRadius) return false;

        Vector3 perpendicular = diff - (projection * normalizedForward);
        float sqrPerpDistance = Vector3.Dot(perpendicular, perpendicular);

        float maxPerpDist = (lineWidth * 0.5f) + targetRadius;
        return sqrPerpDistance <= maxPerpDist * maxPerpDist;
    }

    public static bool IntersectsSphere(Vector3 posA, float radiusA, Vector3 posB, float radiusB)
    {
        Vector3 diff = posA - posB;
        float sqrDist = Vector3.Dot(diff, diff);
        float radiusSum = radiusA + radiusB;
        return sqrDist <= radiusSum * radiusSum;
    }

    public static Vector3[] GenerateConeOutlinePoints(float range, float coneAngleDegrees, int arcSegments = 24)
    {
        Vector3[] points = new Vector3[arcSegments + 3];
        points[0] = Vector3.zero;

        float halfAngle = coneAngleDegrees * 0.5f;
        float startAngle = -halfAngle;
        float step = coneAngleDegrees / arcSegments;

        for (int i = 0; i <= arcSegments; i++)
        {
            float currentAngle = (startAngle + (step * i)) * Mathf.Deg2Rad;

            float x = Mathf.Sin(currentAngle) * range;
            float z = Mathf.Cos(currentAngle) * range;
            points[i + 1] = new Vector3(x, 0f, z);
        }

        points[points.Length - 1] = Vector3.zero;
        return points;
    }

    public static Vector3[] GenerateLineOutlinePoints(float lineLength, float lineWidth)
    {
        float halfWidth = lineWidth * 0.5f;
        return new Vector3[]
        {
            new Vector3(-halfWidth, 0f, 0f),
            new Vector3(-halfWidth, 0f, lineLength),
            new Vector3(halfWidth, 0f, lineLength),
            new Vector3(halfWidth, 0f, 0f),
            new Vector3(-halfWidth, 0f, 0f)
        };
    }
}
