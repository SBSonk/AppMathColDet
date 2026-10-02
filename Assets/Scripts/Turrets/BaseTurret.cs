using System.Collections.Generic;
using UnityEngine;


public abstract class BaseTurret : MonoBehaviour
{
    public static bool isLevelWon;

    [Header("Targeting")]
    [SerializeField] protected float enemyRadius = 0.5f;

    [Header("Visualizer")]
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected Color visualizerColor = new Color(1f, 0.3f, 0.2f, 0.9f);
    [SerializeField] protected float visualizerWidth = 0.08f;

    [Header("Attack Settings")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float fireInterval = 1f;
    [SerializeField] protected float damage = 2;

    protected float _nextFireTime;

    protected virtual void Awake()
    {
        isLevelWon = false;

        if (!firePoint)
        {
            firePoint = transform;
        }

        SetupLineRenderer();
        UpdateRangeVisualizer();
    }

    protected virtual void SetupLineRenderer()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (!lineRenderer)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = visualizerWidth;
        lineRenderer.endWidth = visualizerWidth;
        lineRenderer.startColor = visualizerColor;
        lineRenderer.endColor = visualizerColor;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;

        // Ensure material exists and supports vertex/line coloring
        if (lineRenderer.sharedMaterial == null)
        {
            Shader defaultShader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            if (defaultShader != null)
            {
                lineRenderer.material = new Material(defaultShader) { color = visualizerColor };
            }
        }
    }

    protected virtual void Update()
    {
        if (isLevelWon)
        {
            return;
        }

        if (IsEnemyInDetectionArea())
        {
            OnPlayerDetected();
        }
    }

    protected virtual void OnPlayerDetected()
    {
        if (Time.time >= _nextFireTime)
        {
            Attack();
            _nextFireTime = Time.time + fireInterval;
        }
    }

    public abstract bool IsEnemyInDetectionArea();
    protected abstract void UpdateRangeVisualizer();
    protected abstract void Attack();

    public static void StopAllTurrets()
    {
        isLevelWon = true;
    }

    protected virtual void OnValidate()
    {
        if (lineRenderer != null)
        {
            UpdateRangeVisualizer();
        }
    }
}
