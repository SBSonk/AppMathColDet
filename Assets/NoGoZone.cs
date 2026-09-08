using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NoGoZone : MonoBehaviour
{
    public Transform player;
    public float playerDiameter = 1;
    public float cubeDiameter = 1;
    public float warningDiameter = 1.5f;

    public float shakeIntensity = 1, frequency = 1;

    public Color outColor, warningColor, inColor;

    Renderer _meshRend;
    Material _material;

    bool _enterFlagged, _exitFlagged;
    public UnityEvent IGotEntered, IGotExited;

    Vector3 _startPos;

    public bool badBadZone;
    public float explodeyTime = 3;
    float _inMeTime;

    void Awake()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;

        _meshRend = GetComponent<Renderer>();
        _material = _meshRend.material;

        _startPos = transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * cubeDiameter);
        Gizmos.color = Color.pink;
        Gizmos.DrawWireCube(transform.position, Vector3.one * (cubeDiameter + warningDiameter));
    }

    void Update()
    {
        if (IsPlayerInsideMe(cubeDiameter + warningDiameter))
        {
            if (IsPlayerInsideMe(cubeDiameter))
            {
                _material.color = Color.Lerp(_material.color, inColor, 10 * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, _startPos + ((new Vector3(1, 0, 1) * shakeIntensity) * Mathf.Sin(frequency * Time.deltaTime)), 0.25f);

                if (badBadZone)
                {
                    _inMeTime += Time.deltaTime;

                    if (_inMeTime >= explodeyTime) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            } else
            {
                _material.color = Color.Lerp(_material.color, warningColor, 10 * Time.deltaTime);
                transform.position = _startPos;
                
                _inMeTime = 0;
            }
        } 
        else
        {
            _material.color = Color.Lerp(_material.color, outColor, 10 * Time.deltaTime);
            transform.position = _startPos;

            _inMeTime = 0;
        }
    }
    
    bool IsPlayerInsideMe(float cubeDiameter)
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
