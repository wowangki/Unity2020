using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UI_Weapon
{
    public string weaponName;
    public Sprite weaponImage;
}

public class HUD_Weapon : MonoBehaviour
{
    public GunController GunController { get; set; }
    [SerializeField] private List<UI_Weapon> weaponType = null;
    [SerializeField] private Image curWeaponImage = null;
    [SerializeField] private Text curWeaponName = null;
    [SerializeField] private Text[] bulletText = null;

    private void Update()
    {
        if (!GunController) return;

        CheckGunStatus();
    }

    private void CheckGunStatus()
    {
        curWeaponImage.sprite = weaponType[(int)GunController.CurGun.Kind].weaponImage;
        curWeaponName.text = weaponType[(int)GunController.CurGun.Kind].weaponName;
        bulletText[0].text = GunController.CurGun.CurBulletCount.ToString();
        bulletText[1].text = GunController.CurGun.TotalBulletCount.ToString();
    }
}
