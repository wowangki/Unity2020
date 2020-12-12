using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HUD_GuestCondition : MonoBehaviour, IPointerDownHandler
{
    public NpcInfo TargetInfo { get; set; }
    private Image mGaugeBar = null;
    private Image mCloud = null;
    private Image mItemImg = null;
    [SerializeField]
    private float rotSpeed = 100f;
    private Items mWantItem = null;

    [SerializeField]
    private AudioClip mBuyClip = null;
    [SerializeField]
    private AudioClip mAngryClip = null;
    private AudioSource mAudio = null;

    public delegate void GusetDoDel();
    public event GusetDoDel guestDoHandler;
    public delegate void IncreaseDel(Items item);
    public event IncreaseDel IncreaseHandler;
    public delegate void DecreaseDel(Items value);
    public event DecreaseDel DecreaseHandler;

    public void Init()
    {
        mGaugeBar = transform.Find("Gauge").Find("Bar_Green").GetComponent<Image>();
        mCloud = transform.Find("WantSlot").GetComponent<Image>();
        mItemImg = mCloud.transform.GetChild(0).GetComponent<Image>();
        mAudio = GetComponent<AudioSource>();

        mGaugeBar.fillAmount = 1f;

        Player player = GameObject.Find("Player").GetComponent<Player>();
        IncreaseHandler += player.SellItem;
        DecreaseHandler += player.NoSellItem;
    }

    private void OnEnable()
    {
        mWantItem = GameManager.Instance.ObjPool.GetRandomItem();
        mItemImg.sprite = mWantItem.Sprite;
        StartCoroutine(GaugeCoroutine());
        StartCoroutine(RotCloudCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        TargetInfo = null;

        if (mWantItem)
            mWantItem.ResetItem();
        mWantItem = null;
        StopAllCoroutines();
        mGaugeBar.fillAmount = 1f;
        guestDoHandler = null;
    }

    private IEnumerator GaugeCoroutine()
    {
        while (mGaugeBar.fillAmount > 0f)
        {
            mGaugeBar.fillAmount -= (TargetInfo.AngryValue / 45f) * Time.deltaTime;
            yield return null;
        }

        //Target.IsAngry = true;
        DecreaseHandler(mWantItem);
        if (guestDoHandler != null)
            guestDoHandler();

        StartCoroutine(PlayEndAudioCoroutine(mAngryClip));
    }

    private IEnumerator RotCloudCoroutine()
    {
        while (true)
        {
            mCloud.transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
            mItemImg.transform.Rotate(Vector3.back * rotSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InvenSlot slot = Inventory.GetInvenSlot(mWantItem);
        if (!slot)
        {
            mGaugeBar.fillAmount -= 0.2f;
            return;
        }

        IncreaseHandler(slot.Item);

        if (guestDoHandler != null)
            guestDoHandler();

        StartCoroutine(PlayEndAudioCoroutine(mBuyClip, 0.5f));
    }

    private IEnumerator PlayEndAudioCoroutine(AudioClip clip, float volume = 1f)
    {
        WaitWhile ww = new WaitWhile(() => mAudio.isPlaying);
        mAudio.clip = clip;
        mAudio.volume = volume;
        mAudio.Play();
        SetTotalAlpha(0);
        yield return ww;

        SetTotalAlpha(1f);
        gameObject.SetActive(false);
    }

    private void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void SetTotalAlpha(float a)
    {
        SetAlpha(mGaugeBar, a);
        SetAlpha(mCloud, a);
        SetAlpha(mItemImg, a);
    }
}
