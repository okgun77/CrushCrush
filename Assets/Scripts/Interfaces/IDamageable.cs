using UnityEngine;
using System;

public interface IDamageable
{
    void TakeDamage(int damage);
    void Heal(int amount);
    int GetCurrentHealth();
    int GetMaxHealth();
    bool IsDead();
    event Action<int, int> OnHealthChanged; // 현재체력, 최대체력
    event Action OnDeath;
}
