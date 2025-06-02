using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullHeart;
    public Image[] hearts;

    public PlayerHealth playerHealth;

    private void Update()
    {
        health = playerHealth.currentHealth;
        maxHealth = playerHealth.maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            int heartHealth = Mathf.Clamp(health - (i * 2), 0, 2);

            switch (heartHealth)
            {
                case 2:
                    hearts[i].sprite = fullHeart;
                    break;
                case 1:
                    hearts[i].sprite = halfHeart;
                    break;
                default:
                    hearts[i].sprite = emptyHeart;
                    break;
            }

            hearts[i].enabled = i < (maxHealth + 1) / 2; // supports odd maxHealth
        }
    }

}
