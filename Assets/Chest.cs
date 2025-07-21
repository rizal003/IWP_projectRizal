using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    private bool isOpened = false;
    private SpriteRenderer spriteRenderer;
    public PlayerBuff[] possibleBuffs; // Drag your ScriptableObjects here!

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer && closedSprite)
            spriteRenderer.sprite = closedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.UseKey())
            {
                OpenChest(other.gameObject);
            }
        }
    }

    private void OpenChest(GameObject player)
    {
        isOpened = true;
        if (spriteRenderer && openSprite)
            spriteRenderer.sprite = openSprite;

        GiveReward(player);
    }

    void GiveReward(GameObject player)
    {
        if (possibleBuffs == null || possibleBuffs.Length == 0)
        {
            Debug.LogWarning("No buffs assigned to chest!");
            return;
        }

        // Pick a random buff from the array
        int idx = Random.Range(0, possibleBuffs.Length);
        PlayerBuff buff = possibleBuffs[idx];

        PickupPopup popup = FindObjectOfType<PickupPopup>();
        Debug.Log("Popup found? " + (popup != null));
        if (popup != null)
        {
            string statString = "";
            switch (buff.buffType)
            {
                case PlayerBuff.BuffType.Speed:
                    statString = $"+{buff.buffValue} Speed!";
                    break;
                case PlayerBuff.BuffType.Attack:
                    statString = $"+{buff.buffValue} Attack!";
                    break;
                case PlayerBuff.BuffType.Health:
                    statString = $"+{(int)buff.buffValue} Max Health!";
                    break;
            }
            popup.Show(buff.buffIcon, buff.buffName, statString);
        }
        // Apply the permanent buff effect
        ApplyBuff(player, buff);

        // Update stats UI if present
        StatsPanelUI statsUI = FindObjectOfType<StatsPanelUI>();
        if (statsUI != null)
        {
            statsUI.UpdateStats(); // Update the text

            // Animate only the relevant stat!
            switch (buff.buffType)
            {
                case PlayerBuff.BuffType.Speed:
                    statsUI.AnimateStat(statsUI.moveSpeedText);
                    break;
                case PlayerBuff.BuffType.Attack:
                    statsUI.AnimateStat(statsUI.attackDamageText);
                    break;
                case PlayerBuff.BuffType.Health:
                    statsUI.AnimateStat(statsUI.maxHealthText);
                    break;
            }
        }

        // Optional: Show a quick popup, play SFX, or flash stat UI
        Debug.Log($"{buff.buffName} applied! Value: {buff.buffValue}");
    }

    void ApplyBuff(GameObject player, PlayerBuff buff)
    {
        var stats = player.GetComponent<PlayerStats>();

        switch (buff.buffType)
        {
            case PlayerBuff.BuffType.Speed:
                stats?.IncreaseMoveSpeed(buff.buffValue);
                break;
            case PlayerBuff.BuffType.Attack:
                stats?.IncreaseAttackDamage(buff.buffValue);
                break;
            case PlayerBuff.BuffType.Health:
                stats?.IncreaseMaxHealth((int)buff.buffValue);
                // Optionally heal the player by new max:
                player.GetComponent<PlayerHealth>()?.ChangeHealth((int)buff.buffValue);
                break;
        }
    }
}
