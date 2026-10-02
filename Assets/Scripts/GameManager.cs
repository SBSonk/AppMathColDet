using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;

    [Header("UI Reference")]
    [SerializeField] HealthUI healthUI;

    public Action<float, float> OnHealthChanged;
    public Action OnGameOver;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsGameOver => currentHealth <= 0;

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

        UpdateHealthUI(true);
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

    public void OnEnemyDeath(Enemy enemyStats)
    {
        // TODO: jic
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
