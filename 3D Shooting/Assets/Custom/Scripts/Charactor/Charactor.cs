using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharactorSound
{
    public enum SOUND_KIND
    {
        MOVE,
        HIT
    }

    public string name;
    public SOUND_KIND kind;
    public AudioClip clip;
}

public class Charactor : MonoBehaviour
{
    protected Status status;
    protected Rigidbody rigid;
    protected Animator anim;
    protected GunController gunController;

    protected AudioSource[] audioSources;
    [SerializeField] protected CharactorSound[] audioSounds;

    protected bool isAlive;
    protected Vector3 moveDir;
    protected bool isWalk;
    protected bool isRun;
    protected bool isFineSight;
    [SerializeField] private float changeRunningTime = 0f;
    private float currentMoveTime = 0;

    protected bool isReadyForFire;

    protected virtual void Awake()
    {
        status = GetComponent<Status>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gunController = GetComponentInChildren<GunController>();
        audioSources = GetComponents<AudioSource>();

        status.ApplySpeed = status.WalkSpeed;
    }

    protected virtual void Update()
    {
        CheckMove();
        Move();

        if (isFineSight || isReadyForFire)
            LookShootDir();
        else Turn();
    }

    #region 이동 관련
    protected void Move()
    {
        transform.Translate(moveDir * status.ApplySpeed * Time.deltaTime, Space.World);
    }

    protected virtual void CheckMove()
    {
        if (moveDir.Equals(Vector3.zero))
        {
            currentMoveTime = 0;
            isWalk = false;
            isRun = false;
            status.ApplySpeed = status.WalkSpeed;
            StopSE(0);
        }
        else
        {
            isWalk = true;
            RunTimeCalcul();

            if (!isRun)
                PlaySE("Walk");
            else
            {
                PlaySE("Run");
                status.ApplySpeed = status.RunSpeed;
            }
        }

        anim.SetBool("IsWalk", isWalk);
        anim.SetBool("IsRun", isRun);
    }

    private void RunTimeCalcul()
    {
        if (isRun) return;
        currentMoveTime += Time.deltaTime;

        if(currentMoveTime >= changeRunningTime)
            isRun = true;
    }

    private void Turn()
    {
        if (!isWalk) return;

        Quaternion turnRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              turnRot,
                                              status.ApplySpeed * Time.deltaTime);
    }

    #endregion

    #region 사격 관련
    protected void Fire()
    {
        if (gunController.CurGun.TotalBulletCount > 0 && gunController.CurGun.CurBulletCount <= 0)
        {
            Reload();
            return;
        }
            
        anim.SetBool("IsReadyForFire", isReadyForFire = true);
        anim.SetBool("IsFire", gunController.IsFire);

        gunController.Fire();
    }

    protected void LookShootDir()
    {
        Vector3 lookDir = gunController.Target.position - transform.position;
        lookDir.y = 0;
        
        Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        
        transform.rotation = lookRot;
    }

    protected void Reload()
    {
        if (gunController.IsReload) return;

        gunController.IsReload = true;
        anim.SetTrigger("Reload");

        StartCoroutine(gunController.ReloadCoroutine());
    }

    protected void FireCancel()
    {
        if(!isFineSight)
            anim.SetBool("IsReadyForFire", isReadyForFire = false);

        anim.SetBool("IsFire", gunController.IsFire = false);
        gunController.FireCancel();
    }

    #endregion

    #region 사운드 관련
    private void PlaySE(string soundName)
    {
        for (int i = 0; i < audioSounds.Length; i++)
        {
            if (audioSounds[i].name != soundName) continue;

            switch (audioSounds[i].kind)
            {
                case CharactorSound.SOUND_KIND.MOVE:
                    if (audioSources[0].clip == audioSounds[i].clip) return;
                    audioSources[0].clip = audioSounds[i].clip;
                    audioSources[0].Play();
                    break;
                case CharactorSound.SOUND_KIND.HIT:
                    audioSources[1].clip = audioSounds[i].clip;
                    audioSources[1].Play();
                    break;
            }

            return;
        }
    }

    private void StopSE(int index)
    {
        if (index >= audioSources.Length) return;
        audioSources[index].Stop();
        audioSources[index].clip = null;
    }

    protected void StopAllSE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
            audioSources[i].clip = null;
        }
    }
    #endregion
}
