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
    }
}
