using UnityEngine;
using UnityEngine.SceneManagement;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float radius = 0.35f;
    [SerializeField] float lifetime = 6f;

    Transform _player;
    float _playerRadius = 0.5f;
    Vector3 _velocity;
    float _elapsedTime;
    bool _isInitialized;

    public void Initialize(Vector3 direction, float projSpeed, Transform playerTransform, float playerRad = 0.5f)
    {
        speed = projSpeed;
        _velocity = direction.normalized * speed;
        _player = playerTransform;
        _playerRadius = playerRad;
        _isInitialized = true;
    }

    void Awake()
    {
        if (!_player)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (!_isInitialized && _velocity == Vector3.zero)
        {
            _velocity = transform.forward * speed;
        }

        transform.position += _velocity * Time.deltaTime;
        _elapsedTime += Time.deltaTime;

        if (_player != null)
        {
            if (CollisionHelpers.IntersectsSphere(transform.position, radius, _player.position, _playerRadius))
            {
                HitPlayer();
                return;
            }
        }

        if (_elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void HitPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
