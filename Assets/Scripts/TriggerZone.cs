using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    protected Transform player;
    public float playerDiameter = 1;
    public float cubeDiameter = 1;
    public float warningDiameter = 1.5f;

    public float shakeIntensity = 1, frequency = 1;

    public Color outColor, warningColor, inColor;

    protected Renderer _meshRend;
    protected Material _material;

    protected bool _enterFlagged, _exitFlagged;
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    protected Vector3 _startPos;

    public bool destroyOnEnter;

    protected virtual void Awake()
    {
        if (!player)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        _meshRend = GetComponentInChildren<Renderer>();
        if (_meshRend != null)
        {
            _material = _meshRend.material;
        }

        _startPos = transform.position;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * cubeDiameter);
        Gizmos.color = Color.pink;
        Gizmos.DrawWireCube(transform.position, Vector3.one * (cubeDiameter + warningDiameter));
    }

    protected virtual void Update()
    {
        if (IsPlayerInside(cubeDiameter + warningDiameter))
        {
            if (IsPlayerInside(cubeDiameter))
            {
                OnPlayerInside();
            }
            else
            {
                OnPlayerWarning();
            }
        }
        else
        {
            OnPlayerOutside();
        }
    }

    protected virtual void OnPlayerInside()
    {
        if (_material != null) _material.color = Color.Lerp(_material.color, inColor, 10 * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, _startPos + ((new Vector3(1, 0, 1) * shakeIntensity) * Mathf.Sin(frequency * Time.deltaTime)), 0.25f);
    }

    protected virtual void OnPlayerWarning()
    {
        if (_material != null) _material.color = Color.Lerp(_material.color, warningColor, 10 * Time.deltaTime);
        transform.position = _startPos;
    }

    protected virtual void OnPlayerOutside()
    {
        if (_material != null) _material.color = Color.Lerp(_material.color, outColor, 10 * Time.deltaTime);
        transform.position = _startPos;
    }

    public virtual bool IsPlayerInside(float zoneDiameter)
    {
        if (player == null) return false;

        bool isPlayerInside = false;

        float cubeRadius = zoneDiameter * 0.5f;
        Vector3 minBounds = new Vector3(transform.position.x - cubeRadius, transform.position.y - cubeRadius, transform.position.z - cubeRadius);
        Vector3 maxBounds = new Vector3(transform.position.x + cubeRadius, transform.position.y + cubeRadius, transform.position.z + cubeRadius);

        float playerRadius = playerDiameter * 0.5f;
        if (player.position.x + playerRadius > minBounds.x && player.position.x - playerRadius < maxBounds.x)
        {
            if (player.position.y + playerRadius > minBounds.y && player.position.y - playerRadius < maxBounds.y)
            {
                if (player.position.z + playerRadius > minBounds.z && player.position.z - playerRadius < maxBounds.z)
                {
                    isPlayerInside = true;
                }
            }
        }

        if (isPlayerInside && !_enterFlagged)
        {
            OnPlayerEnter();
        }
        else if (!isPlayerInside && !_exitFlagged)
        {
            OnPlayerExit();
        }

        return isPlayerInside;
    }

    protected virtual void OnPlayerEnter()
    {
        onPlayerEnter?.Invoke();
        _enterFlagged = true;
        _exitFlagged = false;

        if (destroyOnEnter)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnPlayerExit()
    {
        onPlayerExit?.Invoke();
        _exitFlagged = true;
        _enterFlagged = false;
    }
}
