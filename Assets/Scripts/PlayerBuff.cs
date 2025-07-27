using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/PlayerBuff")]
public class PlayerBuff : ScriptableObject
{
    public string buffName;
    public Sprite buffIcon;
    public BuffType buffType;
    public float buffValue = 1f; 

    public enum BuffType { Speed, Attack, Health, SlashSpeed }
}
