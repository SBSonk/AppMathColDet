using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundNumber, enemiesLeft, roundPhase;

    EnemySpawner _enemySpawner;

    void Awake()
    {
        _enemySpawner = GameObject.FindAnyObjectByType<EnemySpawner>();
    }

    void Update()
    {
        roundNumber.SetText($"Round: {_enemySpawner.RoundNumber}");
        enemiesLeft.SetText($"Enemies Left: {_enemySpawner.EnemiesLeft}");
        roundPhase.SetText($"Round State: {_enemySpawner.RoundPhase}");
    }
}
