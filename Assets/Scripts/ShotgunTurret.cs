using UnityEngine;

public class ShotgunTurret : BaseTurret
{
    [Header("Shotgun Cone / Range Settings")]
    [SerializeField] float range = 8f;
    [SerializeField] float coneAngle = 75f;
    [SerializeField] int pelletCount = 5;
    [SerializeField] float pelletSpeed = 12f;

    protected override void Awake()
    {
        fireInterval = 1.8f;
        visualizerColor = new Color(0.8f, 0.2f, 1f, 0.9f);
        base.Awake();
    }

    public override bool IsPlayerInDetectionArea()
    {
        if (player == null) return false;

        return CollisionHelpers.IsPointInConeXZ(transform.position, transform.forward, player.position, range, coneAngle, playerRadius);
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
        if (projectilePrefab == null || pelletCount <= 0) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        float halfAngle = coneAngle * 0.5f;

        // Spread shotgun blast using trigonometry
        for (int i = 0; i < pelletCount; i++)
        {
            float angleDeg = pelletCount > 1 
                ? -halfAngle + (coneAngle / (pelletCount - 1)) * i 
                : 0f;

            float rad = angleDeg * Mathf.Deg2Rad;
            // Local vector: x = sin(rad), z = cos(rad)
            Vector3 localDir = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            Vector3 worldDir = transform.TransformDirection(localDir).normalized;

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(worldDir));
            TurretProjectile proj = projObj.GetComponent<TurretProjectile>();
            if (proj != null)
            {
                proj.Initialize(worldDir, pelletSpeed, player, playerRadius);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3[] points = CollisionHelpers.GenerateConeOutlinePoints(range, coneAngle, 20);
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(transform.TransformPoint(points[i]), transform.TransformPoint(points[i + 1]));
        }
    }
}
