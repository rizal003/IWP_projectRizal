using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float attackDamage = 1f;
    public int maxHealth = 5;
    public float slashSpeed = 10f; // Default slash distance

    // Methods to increase stats
    public void IncreaseMoveSpeed(float amount) => moveSpeed += amount;
    public void IncreaseAttackDamage(float amount) => attackDamage += amount;
    public void IncreaseMaxHealth(int amount) => maxHealth += amount;
    public void IncreaseSlashSpeed(float amount) => slashSpeed += amount;
}


