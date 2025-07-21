using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float attackDamage = 1f;
    public int maxHealth = 5;

    // Methods to increase stats
    public void IncreaseMoveSpeed(float amount) => moveSpeed += amount;
    public void IncreaseAttackDamage(float amount) => attackDamage += amount;
    public void IncreaseMaxHealth(int amount) => maxHealth += amount;
}


