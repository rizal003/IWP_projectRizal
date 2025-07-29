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

    public float hurtPulseScale = 1.2f;
    public float hurtPulseDuration = 0.3f;
    public Color hurtColor = new Color(1, 0.5f, 0.5f);
    void Start()
    {
        RelinkHearts();
    }

    void OnEnable()
    {
        RelinkHearts();
    }

    public void RelinkHearts()
    {
        // Find HeartRow1 and HeartRow2 by name (or tag, or direct reference)
        var row1Obj = GameObject.Find("HeartRow1");
        var row2Obj = GameObject.Find("HeartRow2");

        heartsRow1 = new List<Image>();
        heartsRow2 = new List<Image>();

        if (row1Obj != null)
        {
            heartsRow1.AddRange(row1Obj.GetComponentsInChildren<Image>(true));
        }
        if (row2Obj != null)
        {
            heartsRow2.AddRange(row2Obj.GetComponentsInChildren<Image>(true));
        }
    }


    void Update()
    {
        // Basic null checks to avoid errors if playerHealth or UI is missing/destroyed
        if (playerHealth == null || heartsRow1 == null || heartsRow2 == null)
            return;

        int health = playerHealth.currentHealth;
        int maxHealth = playerHealth.maxHealth;
        int heartsPerRow = 6;
        int totalHearts = Mathf.CeilToInt(maxHealth / 2f); // e.g., 18HP -> 9 hearts

        // Row 1: Fill up to 6 hearts
        for (int i = 0; i < heartsPerRow; i++)
        {
            // Defensive: Check if heart exists and not destroyed
            if (i >= heartsRow1.Count || heartsRow1[i] == null)
                continue;

            if (i < Mathf.Min(totalHearts, heartsPerRow))
            {
                int heartHealth = Mathf.Clamp(health - (i * 2), 0, 2);
                heartsRow1[i].sprite = (heartHealth == 2) ? fullHeart : (heartHealth == 1) ? halfHeart : emptyHeart;
                heartsRow1[i].enabled = true;
            }
            else
            {
                heartsRow1[i].enabled = false;
            }
        }

        // Row 2: Only fill if you have more than 6 hearts!
        for (int i = 0; i < heartsPerRow; i++)
        {
            // Defensive: Check if heart exists and not destroyed
            if (i >= heartsRow2.Count || heartsRow2[i] == null)
                continue;

            int heartIndex = i + heartsPerRow; // 6, 7, 8, ...
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

    public IEnumerator PulseHeart(int heartIndex, bool isRow1)
    {
        List<Image> row = isRow1 ? heartsRow1 : heartsRow2;
        if (heartIndex < 0 || heartIndex >= row.Count || row[heartIndex] == null || !row[heartIndex].enabled)
            yield break; // Don't continue if heart is missing/destroyed

        Image heart = row[heartIndex];
        RectTransform rt = heart.GetComponent<RectTransform>();
        Color originalColor = heart.color;
        Vector3 originalScale = rt.localScale;

        float timer = 0;
        while (timer < hurtPulseDuration)
        {
            // Check again inside loop
            if (heart == null)
                yield break;
            float progress = timer / hurtPulseDuration;
            rt.localScale = originalScale * Mathf.Lerp(hurtPulseScale, 1f, progress);
            heart.color = Color.Lerp(hurtColor, originalColor, progress);
            timer += Time.deltaTime;
            yield return null;
        }
        if (heart == null)
            yield break;

        rt.localScale = originalScale;
        heart.color = originalColor;
    }


}
