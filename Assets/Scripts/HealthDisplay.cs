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
    public List<Image> heartsRow1; 
    public List<Image> heartsRow2;

    void Update()
    {
        int health = playerHealth.currentHealth;
        int maxHealth = playerHealth.maxHealth;
        int heartsPerRow = 10;
        int totalHearts = (maxHealth + 1) / 2; // (e.g. maxHealth 10 -> 5, 12 -> 6, 20 -> 10, 22 -> 11, etc.)

        // Row 1: Fill up to 10 hearts, or all if totalHearts < 10
        for (int i = 0; i < heartsRow1.Count; i++)
        {
            int heartIndex = i;
            if (heartIndex < Mathf.Min(totalHearts, heartsPerRow))
            {
                int heartHealth = Mathf.Clamp(health - (heartIndex * 2), 0, 2);
                heartsRow1[i].sprite = (heartHealth == 2) ? fullHeart : (heartHealth == 1) ? halfHeart : emptyHeart;
                heartsRow1[i].enabled = true;
            }
            else
            {
                heartsRow1[i].enabled = false;
            }
        }

        // Row 2: Only fill if you have more than 10 hearts!
        for (int i = 0; i < heartsRow2.Count; i++)
        {
            int heartIndex = i + heartsPerRow; // 10, 11, 12, ...
            if (heartIndex < totalHearts)
            {
                int heartHealth = Mathf.Clamp(health - (heartIndex * 2), 0, 2);
                heartsRow2[i].sprite = (heartHealth == 2) ? fullHeart : (heartHealth == 1) ? halfHeart : emptyHeart;
                heartsRow2[i].enabled = true;
            }
            else
            {
                heartsRow2[i].enabled = false;
            }
        }
    }
}
