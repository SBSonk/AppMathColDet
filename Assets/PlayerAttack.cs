using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] float holdRadius = 3f;

    [Header("Shooting")]
    [SerializeField] float startingShootAngle = 45;
    [SerializeField] float timeBetweenBullets = 3f;
    [SerializeField] float respawnDelay = 1f;
    [SerializeField] int bulletCount = 4;

    [SerializeField] Transform bulletParent;
    [SerializeField] Bullet bulletPrefab;

    List<Bullet> _queuedBullets = new List<Bullet>();

    float _elapsedTime;
    float _scaledTime;
    float _nextFireTime;

    bool _hasPowerup;
    int _baseBulletCount;

    void Start()
    {
        _baseBulletCount = bulletCount; 

        RegenerateBullets();

        _nextFireTime = timeBetweenBullets;
    }

    void RegenerateBullets()
    {
        CancelInvoke(nameof(RegenerateBullets));

        foreach(Transform child in bulletParent) Destroy(child.gameObject);
        _queuedBullets.Clear();

        for (int i = 0; i < bulletCount; i++)
        {
            _queuedBullets.Add(Instantiate(bulletPrefab, bulletParent));
        }
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        _scaledTime = _elapsedTime * (360 * timeBetweenBullets / bulletCount);

        UpdateBulletPos();
        HandleShooting();
    }

    void UpdateBulletPos()
    {
        for (int i = 0; i < _queuedBullets.Count; i++)
        {
            float _localTime = _scaledTime + (360f / bulletCount) * i;
        
            float xPos = Mathf.Sin(_localTime * Mathf.Deg2Rad);
            float zPos = Mathf.Cos(_localTime * Mathf.Deg2Rad);

            _queuedBullets[i].transform.localPosition = new Vector3(xPos, 0, zPos) * holdRadius;
        }
    }

    void HandleShooting()
    {
        if (Time.time >= _nextFireTime && _queuedBullets.Count > 0)
        {
            for (int i = 0; i < _queuedBullets.Count; i++)
            {
                float currentAngle = (360f / bulletCount) * i + startingShootAngle;

                // Set bullet forward
                _queuedBullets[i].transform.rotation = Quaternion.Euler(0, currentAngle, 0);

                // Shoot da bullet
                _queuedBullets[i].InitBullet(_queuedBullets[i].transform.forward);
                _queuedBullets[i].transform.SetParent(null);
            }

            _queuedBullets.Clear();

            if (_hasPowerup)
            {
                _hasPowerup = false;
                bulletCount = _baseBulletCount; 
            }

            _nextFireTime = Time.time + timeBetweenBullets;
            
            Invoke(nameof(RegenerateBullets), respawnDelay);
        }
    }

    public void GivePowerup(int count)
    {
        _hasPowerup = true;
        bulletCount = count;
        
        RegenerateBullets(); 
    }
}