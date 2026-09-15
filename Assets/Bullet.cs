using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 4;
    [SerializeField] float lifetime = 10;

    Vector3 _velocity;
    float _elapsedTime;
    bool _active = false;

    public void InitBullet(Vector3 dir)
    {
        _velocity = dir * bulletSpeed;

        _active = true;
    }

    void Update()
    {
        if (!_active) return;
        _elapsedTime += Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (_elapsedTime > lifetime) Destroy(gameObject);
    }
}
