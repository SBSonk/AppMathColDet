using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;

    [Header("Coin Settings")]
    [SerializeField] int currentCoins = 0;

    [Header("UI Reference")]
    [SerializeField] HealthUI healthUI;
    [SerializeField] CoinUI coinUI;

    public Action<float, float> OnHealthChanged;
    public Action<int, Vector3> OnCoinsAdded;
    public Action<int> OnCoinsChanged;
    public Action OnGameOver;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsGameOver => currentHealth <= 0;

    public int CurrentCoins => currentCoins;
    public int Coins => currentCoins;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (coinUI == null)
        {
            coinUI = FindAnyObjectByType<CoinUI>();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthUI == null)
        {
            healthUI = FindAnyObjectByType<HealthUI>();
        }

        if (coinUI == null)
        {
            coinUI = FindAnyObjectByType<CoinUI>();
        }

        UpdateHealthUI(true);
        if (coinUI != null)
        {
            coinUI.SubscribeToGameManager();
            coinUI.SetCoinsValueInstant(currentCoins);
        }
    }

    public void AddCoins(int amount, Vector3 worldPosition)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        OnCoinsAdded?.Invoke(amount, worldPosition);
        OnCoinsChanged?.Invoke(currentCoins);
    }

    public void AddCoins(int amount)
    {
        AddCoins(amount, Vector3.zero);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateHealthUI(false);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            HandleGameOver();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI(false);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void OnEnemyReachedEnd(Enemy enemyStats)
    {
        if (enemyStats != null)
        {
            TakeDamage(enemyStats.damage);
        }
    }

    public void OnEnemyDeath(Enemy enemyStats, Vector3 enemyPosition)
    {
        int coinAmount = (enemyStats != null && enemyStats.coins > 0) ? enemyStats.coins : 10;
        AddCoins(coinAmount, enemyPosition);
    }

    public void OnEnemyDeath(Enemy enemyStats)
    {
        OnEnemyDeath(enemyStats, Vector3.zero);
    }

    void UpdateHealthUI(bool instant)
    {
        if (healthUI != null)
        {
            float ratio = maxHealth > 0 ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
            if (instant)
            {
                healthUI.SetHealthValueInstant(ratio);
            }
            else
            {
                healthUI.SetHealthValue(ratio);
            }
        }
    }

    void HandleGameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
    }
}
