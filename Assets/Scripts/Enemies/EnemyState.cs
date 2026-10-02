using System;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [SerializeField] float health = 10;
    [SerializeField] float damage = 10;

    public Action OnDeath;

    public void Initialize(float health, float damage)
    {
        this.health = health;
        this.damage = damage;
    } 

    public void GiveDamage(float amount)
    {
        health -= amount;

        if (health <= 0) HandleDeath();
    }

    public void HandleReachedGoal()
    {
        Destroy(gameObject);
    }

    void HandleDeath()
    {
        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}
