using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    // 지정할 변수
    [SerializeField] private int hp = 0;
    [SerializeField] private int maxHp = 0;
    [SerializeField] private int armor = 0;
    [SerializeField] private int maxArmor = 0;
    [SerializeField] private int oxygen = 0;
    [SerializeField] private int maxOxygen = 0;
    [SerializeField] private float walkSpeed = 0;
    [SerializeField] private float runSpeed = 0;
    [SerializeField] private float breathTime = 0;
    [SerializeField] private float respawnTime = 0;
    [SerializeField] private float invincibleTime = 0;

    // 프로퍼티
    public int Hp { get { return hp; } set { hp = value; } }
    public int MaxHp { get { return maxHp; } }
    public int Armor { get { return armor; } set { armor = value; } }
    public int MaxArmor { get { return maxArmor; } }
    public int Oxygen { get { return oxygen; } set { oxygen = value; } }
    public int MaxOxygen { get { return maxOxygen; } }
    public float WalkSpeed { get { return walkSpeed; } }
    public float RunSpeed { get { return runSpeed; } }
    public float ApplySpeed { get; set; }
    public float BreathTime { get { return breathTime; } set { breathTime = value; } }
    public float RespawnTime { get { return respawnTime; } set { respawnTime = value; } }
    public float InvincibleTime { get { return invincibleTime; } }
    public int Point { get; set; }

}
