using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MenuController
{
    [SerializeField]
    private float animSpeed = 0f;
    private ConstructIcon[] mIconList;

    public override void Init()
    {
        base.Init();

        mIconList = GetComponentsInChildren<ConstructIcon>();

        for (int i = 0; i < mIconList.Length; i++)
        {
            mIconList[i].Init();
            mIconList[i].AnimSpeed = 1 / animSpeed;
        }

        Player pl = GameObject.Find("Player").GetComponent<Player>();
        pl.LoadHandler += LoadData;
    }

    private void LoadData(int shopLv, int forgeLv, int alchmicLv, int sewingLv)
    {
        int _fLv = forgeLv;
        int _aLv = alchmicLv;
        int _sLv = sewingLv;

        for (int i = 0; i < mIconList.Length; i++)
        {
            switch (mIconList[i].Type)
            {
                case ConstructIcon.CONSTRUCT_TYPE.SHOP:
                    mIconList[i].LoadData(shopLv);
                    break;
                case ConstructIcon.CONSTRUCT_TYPE.FORGE:
                    mIconList[i].LoadData(_fLv--);
                    break;
                case ConstructIcon.CONSTRUCT_TYPE.ALCHMIC:
                    mIconList[i].LoadData(_aLv--);
                    break;
                case ConstructIcon.CONSTRUCT_TYPE.SEWING:
                    mIconList[i].LoadData(_sLv--);
                    break;
            }
        }
    }
}
