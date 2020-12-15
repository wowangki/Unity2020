using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructIcon : MonoBehaviour
{
    private Image mImg;
    public Image Cover { get; private set; }

    [SerializeField]
    private Sprite[] mSprits = null;
    private int curId = 0;
    private float currentTime = 0f;
    public float AnimSpeed { get; set; }

    [SerializeField]
    private ConstructIcon parent = null;
    public ConstructIcon Parent { get { return parent; } }
    public bool Activate { get; set; }

    private Text mLvTxt;

    private Text mUpGoldTxt;
    [SerializeField]
    private int mDefaultGold = 0;
    private int mUpGold = 0;
    private AudioSource mAudio = null;

    public enum CONSTRUCT_TYPE
    {
        SHOP,
        FORGE,
        ALCHMIC,
        SEWING,
        NONE
    }

    [SerializeField]
    private CONSTRUCT_TYPE type = CONSTRUCT_TYPE.NONE;
    public CONSTRUCT_TYPE Type { get { return type; } }

    public delegate bool ChkUpgradeAble(CONSTRUCT_TYPE type, int gold);
    public event ChkUpgradeAble UpgradeHandler;

    public void Init()
    {
        mImg = transform.Find("Img_Icon").GetComponent<Image>();

        if (type == CONSTRUCT_TYPE.SHOP)
        {
            mLvTxt = transform.Find("LvTxt").Find("Text_LvStr").GetComponent<Text>();
            mLvTxt.text = Player.ShopLv.ToString();
            Activate = true;
        }
        else
            Cover = transform.Find("UseableCover").GetComponent<Image>();

        mUpGold = mDefaultGold;
        mUpGoldTxt = transform.Find("UpTxt").GetComponentInChildren<Text>();
        mUpGoldTxt.text = mUpGold.ToString();
        mAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(AnimCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator AnimCoroutine()
    {
        if (Cover)
        {
            WaitWhile ww = new WaitWhile(() => Cover.gameObject.activeSelf);
            yield return ww;
        }

        while (true)
        {
            currentTime += Time.deltaTime;
            if(currentTime >= AnimSpeed)
            {
                curId++;

                if (mSprits.Length <= curId)
                    curId = 0;

                mImg.sprite = mSprits[curId];
                currentTime = 0f;
            }

            yield return null;
        }
    }

    public void Activating()
    {
        if(type == CONSTRUCT_TYPE.SHOP)
        {
            if (!UpgradeHandler(type, mUpGold)) return;
            mUpGold += 100 * Player.ShopLv;
            mLvTxt.text = Player.ShopLv.ToString();
            mUpGoldTxt.text = mUpGold.ToString();
            mAudio.Play();
        }
        else
        {
            if (Activate || !parent.Activate) return;
            if (!UpgradeHandler(type, mUpGold)) return;
            Activate = true;
            Cover.gameObject.SetActive(false);
            mAudio.Play();
        }
    }

    public void LoadData(int lv)
    {
        if(type == CONSTRUCT_TYPE.SHOP)
        {
            mUpGold = mDefaultGold;
            for (int i = 1; i < lv; i++)
            {
                mUpGold += 100 * i;
            }

            mLvTxt.text = lv.ToString();
            mUpGoldTxt.text = mUpGold.ToString();
        }
        else
        {
            Activate = false;
            Cover.gameObject.SetActive(true);
            if (lv <= 0) return;

            Activate = true;
            Cover.gameObject.SetActive(false);
        }
    }
}
