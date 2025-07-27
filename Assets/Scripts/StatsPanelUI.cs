using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanelUI : MonoBehaviour
{
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackDamageText;
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI slashSpeedText;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        UpdateStats();
    }

    public void UpdateStats()
    {
        moveSpeedText.text = playerStats.moveSpeed.ToString("0.00");
        attackDamageText.text = playerStats.attackDamage.ToString("0.00");
        maxHealthText.text = playerStats.maxHealth.ToString("0.00");
        slashSpeedText.text = playerStats.slashSpeed.ToString("0.00");
    }
    public void AnimateStat(TextMeshProUGUI statText)
    {
        StartCoroutine(StatPopCoroutine(statText));
    }

    IEnumerator StatPopCoroutine(TextMeshProUGUI statText)
    {
        Color originalColor = statText.color;
        Vector3 originalScale = statText.transform.localScale;

        statText.color = Color.yellow;
        statText.transform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.15f);

      
        float t = 0;
        float duration = 0.4f;
        while (t < duration)
        {
            statText.transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t / duration);
            statText.color = Color.Lerp(Color.yellow, originalColor, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        statText.transform.localScale = originalScale;
        statText.color = originalColor;
    }


}


