using UnityEngine;
using Helpers;

public class FlameTurret : BaseTurret
{
    [Header("Flame Cone Settings")]
    [SerializeField] float range = 7f;
    [SerializeField] float coneAngle = 60f;
    [SerializeField] float projectileSpeed = 8f;

    public override bool IsEnemyInDetectionArea()
    {
        foreach (GameObject g in EnemySpawner.Instance.ActiveEnemyObjects)
        {
            if (CollisionHelpers.IsPointInConeXZ(transform.position, transform.forward, g.transform.position, range, coneAngle, enemyRadius)) return true;
        }

        return false;
    }

    protected override void UpdateRangeVisualizer()
    {
        if (lineRenderer == null) return;

        Vector3[] outlinePoints = CollisionHelpers.GenerateConeOutlinePoints(range, coneAngle, 30);
        lineRenderer.positionCount = outlinePoints.Length;
        lineRenderer.SetPositions(outlinePoints);
    }

    protected override void Attack()
    {
        SpawnFlameParticle();
    }

    void SpawnFlameParticle()
    {
        if (projectilePrefab == null) return;

        float halfAngle = coneAngle * 0.5f;
        float randomAngleDeg = Random.Range(-halfAngle, halfAngle);
        float rad = randomAngleDeg * Mathf.Deg2Rad;

        Vector3 localDir = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
        Vector3 worldDir = transform.TransformDirection(localDir).normalized;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(worldDir));
        TurretProjectile proj = projObj.GetComponent<TurretProjectile>();
        if (proj != null)
        {
            proj.Initialize(worldDir, projectileSpeed, enemyRadius, damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3[] points = CollisionHelpers.GenerateConeOutlinePoints(range, coneAngle, 20);
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(transform.TransformPoint(points[i]), transform.TransformPoint(points[i + 1]));
        }
    }
}
