using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static public int ShopLv { get; private set; } = 1;
    static public int ForgeLv { get; private set; } = 0;
    static public int AlchmicLv { get; private set; } = 0;
    static public int SewingLv { get; private set; } = 0;

    private int mMyGold = 10000;
    private const int mMaxGold = 999999999;
    private int mMyRepute = 0;
    private const int mMaxRepute = 999999999;

    public delegate void ShowGold(int value);
    public event ShowGold GoldUIHandler;
    public delegate void ShowRepute(int value);
    public event ShowRepute ReputeUIHandler;

    private void Awake()
    {
        ConstructIcon[] icon = GameObject.Find("Menu").transform.Find("Construct").GetComponentsInChildren<ConstructIcon>();

        for (int i = 0; i < icon.Length; i++)
        {
            icon[i].UpgradeHandler += UpgradeShop;
        }
    }

    private void Start()
    {
        GoldUIHandler(mMyGold);
        ReputeUIHandler(mMyRepute);
    }

    public void SellItem(Items items)
    {
        mMyGold += Mathf.RoundToInt(items.Tier != 0 ? (items.Value + mMyRepute) * items.Tier * 1.5f * 100 : (items.Value + mMyRepute) * 1.5f * 100);   
        mMyRepute += Mathf.RoundToInt(items.Tier != 0 ? items.Value * items.Tier * 1.5f : items.Value * 1.5f);

        if (mMyGold >= mMaxGold)
            mMyGold = mMaxGold;
        if (mMyRepute >= mMaxRepute)
            mMyRepute = mMaxRepute;

        GoldUIHandler(mMyGold);
        ReputeUIHandler(mMyRepute);
    }

    public void NoSellItem(Items items)
    {
        mMyRepute -= Mathf.RoundToInt(items.Tier != 0 ? (items.Tier + 1) * 25 : 25);

        if (mMyRepute < 0)
            mMyRepute = 0;

        ReputeUIHandler(mMyRepute);
    }

    private bool UpgradeShop(ConstructIcon.CONSTRUCT_TYPE type, int useGold)
    {
        if (mMyGold < useGold) return false;
        mMyGold -= useGold;
        GoldUIHandler(mMyGold);
        switch (type)
        {
            case ConstructIcon.CONSTRUCT_TYPE.SHOP:
                {
                    ShopLv++;
                    GameManager.Instance.ObjPool.UnLockItem(CraftMenu.CRAFT_TYPE.NONE, Mathf.CeilToInt(ShopLv / 5f));
                }
                break;
            case ConstructIcon.CONSTRUCT_TYPE.FORGE:
                {
                    ForgeLv++;
                    GameManager.Instance.ObjPool.UnLockItem(CraftMenu.CRAFT_TYPE.FORGE, ForgeLv);
                }
                break;
            case ConstructIcon.CONSTRUCT_TYPE.ALCHMIC:
                {
                    AlchmicLv++;
                    GameManager.Instance.ObjPool.UnLockItem(CraftMenu.CRAFT_TYPE.ALCHMIC, AlchmicLv);
                }
                break;
            case ConstructIcon.CONSTRUCT_TYPE.SEWING:
                {
                    SewingLv++;
                    GameManager.Instance.ObjPool.UnLockItem(CraftMenu.CRAFT_TYPE.SEWING, SewingLv);
                }
                break;
            default:
            case ConstructIcon.CONSTRUCT_TYPE.NONE:
                {
                    Debug.Log("건설 타입을 지정하세요 : " + gameObject.name);
                    return false;
                }
        }

        return true;
    }
}
