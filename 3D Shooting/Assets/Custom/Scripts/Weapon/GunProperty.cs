using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunProperty", menuName = "Scriptable Object/Weapon/Gun", order = int.MaxValue)]
public class GunProperty : ScriptableObject
{
    public enum GUN_KIND
    {
        NORMAL,
        GRENADE,
        SNIPE
    }

    public enum SHOOT_TYPE
    {
        SINGLE,
        BURST,
        AUTO
    }

    //Variable
    [SerializeField] private bool isActivated = false;                  // 총 활성화 여부 (사용 가능 여부)

    [SerializeField] private GUN_KIND kind = GUN_KIND.NORMAL;           // 총의 종류
    [SerializeField] private SHOOT_TYPE shootType = SHOOT_TYPE.SINGLE;  // 발사 방식 (단발, 점사, 연사)

    [SerializeField] private int range = 0;                             // 사정거리
    [SerializeField] private float[] fireRate = null;                   // 발사 간격

    [SerializeField] private int curBulletCount = 0;                    // 현재 탄창의 남은 탄 수
    [SerializeField] private int totalBulletCount = 0;                  // 총 보유 탄
    [SerializeField] private int reloadBulletCount = 0;                 // 재장전 탄 수
    [SerializeField] private int[] maxShootBulletCount = null;          // 최대 발사할 탄 수
    [SerializeField] private float retroActionForce = 0f;               // 발사시 반동

    [SerializeField] private float idleAccuary = 0f;                    // 정지시 정확도
    [SerializeField] private float walkAccuary = 0f;                    // 걷을 때 정확도
    [SerializeField] private float runAccuary = 0f;                     // 뛸 때 정확도
    [SerializeField] private float snipeAccuary = 0f;                   // 조준 시 정확도 (저격총에만 사용)

    public bool IsActivated { get { return isActivated; } set { isActivated = value; } }

    public GUN_KIND Kind { get { return kind; } set { kind = value; } }
    public SHOOT_TYPE ShootType { get { return shootType; } set { shootType = value; } }

    public int Range { get { return range; } }

    public float CurrentFireRate { get; set; }
    public float[] FireRate { get { return fireRate; } }

    public int CurBulletCount { get { return curBulletCount; } set { curBulletCount = value; } }
    public int TotalBulletCount { get { return totalBulletCount; } set { totalBulletCount = value; } }
    public int ReloadBulletCount { get { return reloadBulletCount; } }
    public int[] MaxShootBulletCount { get { return maxShootBulletCount; } }
    public int CurMaxShootBulletCount { get; set; }

    public float RetroActionForce { get { return retroActionForce; } }

    public float ApplyAccuary { get; set; }
    public float IdleAccuary { get { return idleAccuary; } }
    public float WalkAccuary { get { return walkAccuary; } }
    public float RunAccuary { get { return runAccuary; } }
    public float SnipeAccuary { get { return snipeAccuary; } }

    public AudioClip[] clip;
}
