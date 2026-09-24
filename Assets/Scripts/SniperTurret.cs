using UnityEngine;

public class SniperTurret : BaseTurret
{
    [Header("Sniper Line of Sight Settings")]
    [SerializeField] float lineLength = 25f;
    [SerializeField] float lineWidth = 1.0f;
    [SerializeField] float bulletSpeed = 25f;

    bool _wasPlayerInSight;

    protected override void Awake()
    {
        fireInterval = 1.2f;
        visualizerColor = new Color(0.2f, 0.8f, 1f, 0.9f);
        base.Awake();
    }

    public override bool IsPlayerInDetectionArea()
    {
        if (player == null) return false;

        return CollisionHelpers.IsPointInLineSegmentXZ(transform.position, transform.forward, player.position, lineLength, lineWidth, playerRadius);
    }

    protected override void UpdateRangeVisualizer()
    {
        if (lineRenderer == null) return;

        Vector3[] outlinePoints = CollisionHelpers.GenerateLineOutlinePoints(lineLength, lineWidth);
        lineRenderer.positionCount = outlinePoints.Length;
        lineRenderer.SetPositions(outlinePoints);
    }

    protected override void OnPlayerDetected()
    {
        if (!_wasPlayerInSight && Time.time >= _nextFireTime)
        {
            Attack();
            _nextFireTime = Time.time + fireInterval;
        }

        _wasPlayerInSight = true;
    }

    protected override void Update()
    {
        if (isLevelWon || player == null) return;

        bool inSight = IsPlayerInDetectionArea();
        if (inSight)
        {
            OnPlayerDetected();
        }
        else
        {
            _wasPlayerInSight = false;
        }
    }

    protected override void Attack()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 shootDir = transform.forward;

        GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(shootDir));
        TurretProjectile proj = projObj.GetComponent<TurretProjectile>();
        if (proj != null)
        {
            proj.Initialize(shootDir, bulletSpeed, player, playerRadius);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3[] points = CollisionHelpers.GenerateLineOutlinePoints(lineLength, lineWidth);
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(transform.TransformPoint(points[i]), transform.TransformPoint(points[i + 1]));
        }
    }
}
