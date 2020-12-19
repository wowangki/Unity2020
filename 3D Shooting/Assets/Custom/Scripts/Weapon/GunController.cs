using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Gun_Sounds
{
    public string name = null;
    public AudioClip clip = null;
}

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun myGun = null;
    [SerializeField] private GunProperty curGun;
    public GunProperty CurGun { get { return curGun; } set { curGun = value; } }

    // 컨트롤러 관련 내용
    [SerializeField] private Transform fireTr = null;
    public Transform Target { get; set; }

    public bool IsFire { get; set; }
    public bool IsReload { get; set; }
    [SerializeField] private float reloadTime = 0f;

    private AudioSource[] audioSources;
    [SerializeField] private Gun_Sounds[] audioSounds = null;
    [SerializeField] private ParticleSystem muzzleFlash = null;
    private Coroutine shootCoroutine;
    
    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        myGun.RegistGun();
        CurGun = myGun.CurProperty;
    }

    private void Update()
    {
        //Debug.DrawLine(fireTr.position, Target.position, Color.red);

        GunFireRateCalcul();
    }

    #region 발사 관련
    private void GunFireRateCalcul()
    {
        if (IsFire) return;

        if (CurGun.CurrentFireRate > 0)
            CurGun.CurrentFireRate -= Time.deltaTime;

        if (CurGun.CurrentFireRate <= 0)
            IsFire = true;
    }

    public void Fire()
    {
        if (!IsFire) return;

        int shootType = (int)CurGun.ShootType;
        CurGun.CurrentFireRate = CurGun.FireRate[shootType];
        
        if (CurGun.CurBulletCount <= 0)
        {
            PlaySE("Empty", 0.5f);
            return;
        }
        else shootCoroutine = StartCoroutine(ShootBullet());

        IsFire = false;
    }

    private IEnumerator ShootBullet()
    {
        WaitForSeconds ws = new WaitForSeconds(0.1f);
        int t_Count = 0;

        while (t_Count < CurGun.CurMaxShootBulletCount)
        {
            GameObject t_Bullet = ObjectPool.Instance.AssignBullet(CurGun.Kind);
            Bullet bullet = t_Bullet.GetComponent<Bullet>();
            if (!bullet) yield break;

            bullet.transform.position = fireTr.position;
            bullet.transform.rotation = fireTr.rotation;
            bullet.Range = CurGun.Range;
            bullet.FirePos = fireTr.position;
            bullet.Fire(Target.position);
            CameraManager.Instance.curCam.GetComponent<CameraController>().ShakeCam(CurGun.RetroActionForce);
            ++t_Count;
            CurGun.CurBulletCount--;
            muzzleFlash.Play();
            PlayShootSound(CurGun.ShootType);
            yield return ws;
        }
    }

    public void FireCancel()
    {
        IsFire = false;
        if(shootCoroutine != null)
            StopCoroutine(shootCoroutine);
    }


    #endregion

    #region 무기 변경 관련

    public void ChangeGun(GunProperty.GUN_KIND kind)
    {
        myGun.ChangeGun(kind);
        CurGun = myGun.CurProperty;
    }

    public void ChangeShootType()
    {
        myGun.ChangeShootType();
    }

    #endregion

    #region 재장전
    public IEnumerator ReloadCoroutine()
    {
        WaitForSeconds ws = new WaitForSeconds(reloadTime);
        CurGun.TotalBulletCount += CurGun.CurBulletCount;
        CurGun.CurBulletCount = 0;

        PlaySE("Reload", 0.5f);
        yield return ws;

        if (CurGun.TotalBulletCount >= CurGun.ReloadBulletCount)
        {
            CurGun.CurBulletCount = CurGun.ReloadBulletCount;
            CurGun.TotalBulletCount -= CurGun.ReloadBulletCount;
        }
        else
        {
            CurGun.CurBulletCount = CurGun.TotalBulletCount;
            CurGun.TotalBulletCount = 0;
        }

        IsReload = false;
        yield break;
    }
    #endregion

    #region 사운드 관련
    private AudioSource GetRestAudioSource()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].isPlaying) continue;

            return audioSources[i];
        }

        return null;
    }
    
    private void PlayShootSound(GunProperty.SHOOT_TYPE type, float volume = 1f)
    {
        AudioSource audio = GetRestAudioSource();
        if (!audio) return;

        audio.clip = CurGun.clip[(int)type];
        audio.volume = volume;
        audio.Play();
    }

    private void PlaySE(string name, float volume = 1f)
    {
        AudioSource audio = GetRestAudioSource();
        if (!audio)
        {
            StopSE(0);
            PlaySE(name, volume);
            return;
        }

        for (int i = 0; i < audioSounds.Length; i++)
        {
            if (audioSounds[i].name != name) continue;
            audio.clip = audioSounds[i].clip;
            audio.Play();
            return;
        }
    }

    private void StopSE(int index)
    {
        audioSources[index].Stop();
    }

    private void StopAllSE()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
        }
    }

    #endregion
}
