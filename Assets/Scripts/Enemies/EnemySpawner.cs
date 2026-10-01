using System;
using System.Collections;
using Helpers;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
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

    int _enemiesLeftInWave;
    bool _isReady;
    int _roundIndex = 0;
    RoundState _roundState = RoundState.NotStarted;

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

            // Spawn Enemies
            var currentWave = waves[_roundIndex];
            for (int i = 0; i < currentWave.amount; i++)
            {
                var newGO = Instantiate(currentWave.enemy.prefab);
                InitializeEnemy(newGO, currentWave.enemy);

                _enemiesLeftInWave++;

                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }

            _roundState = RoundState.Ongoing;
            yield return new WaitUntil(() => _enemiesLeftInWave == 0);

            _roundState = RoundState.PostRound;
            yield return new WaitForSeconds(roundEndBufferTime);

            OnWaveComplete?.Invoke(_roundIndex);
            _roundIndex++;
        }
    }

    void InitializeEnemy(GameObject enemy, Enemy enemyStats)
    {
        enemy.AddComponent<SplineFollower>();

        if (enemy.TryGetComponent<SplineFollower>(out var follower))
        {
            follower.SetSpline(mapSpline);
            follower.SetSpeed(enemyStats.speed);
            follower.SetEase(enemyStats.easeType);

            // connect death events
            follower.OnReachEnd += () =>
            {
                Destroy(enemy);
                _enemiesLeftInWave--;
            };
        }
    }

    public int RoundNumber => _roundIndex + 1;
    public int EnemiesLeft => _enemiesLeftInWave;
    public string RoundPhase => _roundState.ToString();
}
