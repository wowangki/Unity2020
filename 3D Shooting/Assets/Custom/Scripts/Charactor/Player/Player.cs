using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Charactor
{
    [SerializeField] private CameraController myCam = null;
    private CrossHair crossHair;
    private HUD_Status UIStatus;
    private HUD_Weapon UIWeapon;

    private void Start()
    {
        CameraManager.Instance.SetTarget(transform);
        myCam = CameraManager.Instance.curCam.GetComponent<CameraController>();
        gunController.Target = myCam.shootTr;
        if (myCam == null)
            Debug.Log("카메라 컨트롤러 없음");

        crossHair = FindObjectOfType<CrossHair>();
        UIStatus = FindObjectOfType<HUD_Status>();
        UIWeapon = FindObjectOfType<HUD_Weapon>();

        UIStatus.InitGauge(status);
        UIWeapon.GunController = gunController;
    }

    protected override void Update()
    {
        base.Update();
        MoveControll();
        WeaponControll();
    }

    #region 이동 조작 관련
    private void MoveControll()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 t_moveDir = (h * myCam.transform.right + v * myCam.transform.forward);
        t_moveDir.y = 0;
        moveDir = t_moveDir.normalized;

        Vector3 lookDir = new Vector3(h * Mathf.Abs(transform.position.x), 0, v * Mathf.Abs(transform.position.z)).normalized;
        anim.SetFloat("H", lookDir.x);
        anim.SetFloat("V", lookDir.z);

    }
    #endregion

    #region 무기 조작 관련
    private void WeaponControll()
    {
        FireControll();
        ReloadControll();
        FineSightControll();
        ChangeGun();
        ChangeShootType();
    }

    private void FireControll()
    {
        if (Input.GetMouseButtonUp(0))
        {
            FireCancel();
        }

        if (Input.GetMouseButton(0))
        {
            LookShootDir();
            Fire();
        }
    }

    private void ReloadControll()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    private void FineSightControll()
    {
        if (Input.GetMouseButtonDown(1) && !gunController.IsReload)
        {
            isFineSight = !isFineSight;
            if(!isReadyForFire) isReadyForFire = true;
            anim.SetBool("IsReadyForFire", isReadyForFire);

            if (isFineSight)
                CameraManager.Instance.ChangeCamera((int)gunController.CurGun.Kind + 1);
            else
                CameraManager.Instance.ChangeCamera(0);

            CameraManager.Instance.SetTarget(transform);
            CameraController temp = CameraManager.Instance.curCam.GetComponent<CameraController>();
            if (temp)
            {
                myCam = temp;
                gunController.Target = myCam.shootTr;
            }
        }
    }


    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (gunController.CurGun.Kind == GunProperty.GUN_KIND.NORMAL) return;

            gunController.ChangeGun(GunProperty.GUN_KIND.NORMAL);
            crossHair.ChangeGunType(GunProperty.GUN_KIND.NORMAL);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (gunController.CurGun.Kind == GunProperty.GUN_KIND.GRENADE) return;

            gunController.ChangeGun(GunProperty.GUN_KIND.GRENADE);
            crossHair.ChangeGunType(GunProperty.GUN_KIND.GRENADE);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (gunController.CurGun.Kind == GunProperty.GUN_KIND.SNIPE) return;

            gunController.ChangeGun(GunProperty.GUN_KIND.SNIPE);
            crossHair.ChangeGunType(GunProperty.GUN_KIND.SNIPE);
        }
    }

    private void ChangeShootType()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            gunController.ChangeShootType();
            anim.SetInteger("ShootType", (int)gunController.CurGun.ShootType);
        }
    }

    #endregion
}
