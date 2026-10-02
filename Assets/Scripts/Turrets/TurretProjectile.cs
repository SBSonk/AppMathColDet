using UnityEngine;
using Helpers;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float radius = 0.35f;
    [SerializeField] float lifetime = 6f;

    float damage;
    float _hitRadius = 0.5f;
    Vector3 _velocity;
    float _elapsedTime;
    bool _isInitialized;

    public void Initialize(Vector3 direction, float projSpeed, float playerRad = 0.5f, float damage = 1)
    {
        this.damage = damage;
        speed = projSpeed;
        _velocity = direction.normalized * speed;
        _hitRadius = playerRad;
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized && _velocity == Vector3.zero)
        {
            _velocity = transform.forward * speed;
        }

        transform.position += _velocity * Time.deltaTime;
        _elapsedTime += Time.deltaTime;

        foreach (GameObject g in EnemySpawner.Instance.ActiveEnemyObjects)
        {
            if (CollisionHelpers.IntersectsSphere(transform.position, radius, g.transform.position, _hitRadius))
            {
                // TODO: replace with enemy health
                if (g.TryGetComponent<EnemyState>(out var state)) state.GiveDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        if (_elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
