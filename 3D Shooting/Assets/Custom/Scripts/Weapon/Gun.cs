using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunProperty[] properties = null;
    private Dictionary<GunProperty.GUN_KIND, GunProperty> propertieList = new Dictionary<GunProperty.GUN_KIND, GunProperty>();
    public GunProperty CurProperty { get; set; }

    #region 총 리스트 관련 함수
    public void RegistGun()
    {
        for (int i = 0; i < properties.Length; i++)
        {
            GunProperty t_Prefab = Instantiate(properties[i]);
            t_Prefab.CurrentFireRate = 0;
            t_Prefab.CurMaxShootBulletCount = properties[i].MaxShootBulletCount[0];
            t_Prefab.ShootType = GunProperty.SHOOT_TYPE.SINGLE;
            t_Prefab.ApplyAccuary = properties[i].IdleAccuary;
            t_Prefab.IsActivated = true;
            propertieList.Add(properties[i].Kind, t_Prefab);
        }

        CurProperty = propertieList[GunProperty.GUN_KIND.NORMAL];
        CurProperty.IsActivated = true;
    }

    public void ChangeGun(GunProperty.GUN_KIND kind)
    {
        if (!propertieList[kind].IsActivated) return;
        CurProperty = propertieList[kind];
    }

    public void ChangeShootType()
    {
        if (CurProperty.MaxShootBulletCount.Length < 1) return;

        if (CurProperty.ShootType < GunProperty.SHOOT_TYPE.AUTO)
            ++CurProperty.ShootType;
        else
            CurProperty.ShootType = GunProperty.SHOOT_TYPE.SINGLE;

        CurProperty.CurMaxShootBulletCount = CurProperty.MaxShootBulletCount[(int)CurProperty.ShootType];
    }

    public void ActivateGun(GunProperty.GUN_KIND kind)
    {
        propertieList[kind].IsActivated = true;
    }

    public void DeActivateGun(GunProperty.GUN_KIND kind)
    {
        propertieList[kind].IsActivated = false;
    }
    #endregion
}
