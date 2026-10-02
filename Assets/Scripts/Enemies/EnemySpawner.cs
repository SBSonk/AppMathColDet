using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Serializable]
    public class EnemyWave
    {
        public Enemy enemy;
        public int amount;
        public float timeBetweenSpawns = 0.5f;
    }

    enum RoundState { NotStarted, PreRound, Ongoing, PostRound }

    [Header("Rounds")]
    [SerializeField] EnemyWave[] waves;
    [SerializeField, Tooltip("Time before the round is considered over after the last enemy dies/reaches the goal.")] float roundEndBufferTime = 3; 

    [Header("References")]
    [SerializeField] Spline mapSpline;

    public Action<int> OnWaveComplete;
    public Action<Enemy> OnEnemyDeath;
    public Action<Enemy, Vector3> OnEnemyDeathWithPosition;
    public Action<Enemy> OnEnemyReachedEnd;

    List<GameObject> _activeEnemies = new List<GameObject>();
    bool _isReady;
    int _roundIndex = 0;
    RoundState _roundState = RoundState.NotStarted;

    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        StartCoroutine(StartWaveLoop());
    }

    void Update()
    {
        if (_roundState == RoundState.PreRound && Input.GetKeyDown(KeyCode.Return)) MarkReady();
    }

    void MarkReady() => _isReady = true;

    IEnumerator StartWaveLoop()
    {
        while (_roundIndex < waves.Length)
        {
            _roundState = RoundState.PreRound;
            _isReady = false;
            yield return new WaitUntil(() => _isReady);

            _roundState = RoundState.Ongoing;

            // Spawn Enemies
            var currentWave = waves[_roundIndex];
            for (int i = 0; i < currentWave.amount; i++)
            {
                var newGO = Instantiate(currentWave.enemy.prefab);
                InitializeEnemy(newGO, currentWave.enemy);

                _activeEnemies.Add(newGO);

                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }

            yield return new WaitUntil(() => _activeEnemies.Count == 0); // TODO: handle death state

            _roundState = RoundState.PostRound;
            yield return new WaitForSeconds(roundEndBufferTime);

            OnWaveComplete?.Invoke(_roundIndex);
            _roundIndex++;
        }

        // you win!
    }

    void InitializeEnemy(GameObject enemy, Enemy enemyStats)
    {
        var follower = enemy.GetComponent<SplineFollower>();
        if (follower == null) follower = enemy.AddComponent<SplineFollower>();

        var status = enemy.GetComponent<EnemyState>();
        if (status == null) status = enemy.AddComponent<EnemyState>();

        follower.SetSpline(mapSpline);
        follower.SetSpeed(enemyStats.speed);
        follower.SetEase(enemyStats.easeType);

        // connect death events
        follower.OnReachEnd += () =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyReachedEnd(enemyStats);
            }
            OnEnemyReachedEnd?.Invoke(enemyStats);
            status.HandleReachedGoal();
            _activeEnemies.Remove(enemy);
        };

        status.Initialize(enemyStats.health, enemyStats.damage);
        status.OnDeath += () =>
        {
            Vector3 deathPosition = enemy != null ? enemy.transform.position : Vector3.zero;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyDeath(enemyStats, deathPosition);
            }
            OnEnemyDeath?.Invoke(enemyStats);
            OnEnemyDeathWithPosition?.Invoke(enemyStats, deathPosition);
            _activeEnemies.Remove(enemy);
        };
    }

    public List<GameObject> ActiveEnemyObjects => _activeEnemies;
    public int RoundNumber => _roundIndex + 1;
    public int EnemiesLeft => _activeEnemies.Count;
    public string RoundPhase => _roundState.ToString();
}
