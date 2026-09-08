using UnityEngine;
using UnityEngine.Events;

public class NoGoZone : MonoBehaviour
{
    public Transform player;
    public float playerDiameter = 1;
    public float cubeDiameter = 1;

    public Color outColor, inColor;

    Renderer _meshRend;
    Material _material;

    bool _enterFlagged, _exitFlagged;
    public UnityEvent IGotEntered, IGotExited;

    void Awake()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;

        _meshRend = GetComponent<Renderer>();
        _material = _meshRend.material;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * cubeDiameter);
    }

    void Update()
    {
        _material.color = Color.Lerp(_material.color, IsPlayerInsideMe() ? inColor : outColor, 10 * Time.deltaTime);
    }
    
    bool IsPlayerInsideMe()
    {
        bool HesInsideMe = false;

        float cubeRad = cubeDiameter * 0.5f;
        Vector3 minBounds = new Vector3(transform.position.x - cubeRad, transform.position.y - cubeRad, transform.position.z - cubeRad);
        Vector3 maxBounds = new Vector3(transform.position.x + cubeRad, transform.position.y + cubeRad, transform.position.z + cubeRad);

        float theRadderPlayer = playerDiameter * 0.5f;
        if (player.position.x + theRadderPlayer > minBounds.x && player.position.x - theRadderPlayer < maxBounds.x)
        {
            if (player.position.y + theRadderPlayer > minBounds.y && player.position.y - theRadderPlayer < maxBounds.y)
            {
                if (player.position.z + theRadderPlayer > minBounds.z && player.position.z - theRadderPlayer < maxBounds.z)
                {
                    HesInsideMe = true;
                }
            }
        }

        if (HesInsideMe && !_enterFlagged)
        {
            IGotEntered?.Invoke();
            _enterFlagged = true;
            _exitFlagged = false;
        } else if (!HesInsideMe && !_exitFlagged)
        {
            IGotExited?.Invoke();
            _exitFlagged = true;
            _enterFlagged = false;
        }

        return HesInsideMe;
    }
}
