using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Item Pool
    [SerializeField]
    private List<Items> mItemList = new List<Items>();
    private Dictionary<Items.ITEM_TYPE, Dictionary<int, Dictionary<string, List<Items>>>> mItemPool = new Dictionary<Items.ITEM_TYPE, Dictionary<int, Dictionary<string, List<Items>>>>();
    private Dictionary<Items.ITEM_TYPE, Dictionary<int, List<string>>> mItemKey = new Dictionary<Items.ITEM_TYPE, Dictionary<int, List<string>>>();
    private int maxItemCount = 500;
    #endregion

    #region Npc Pool

    [SerializeField]
    private List<NpcInfo> mNpcInfo = new List<NpcInfo>();
    private Dictionary<string, List<GameObject>> mNpcPool = new Dictionary<string, List<GameObject>>();
    private List<string> mNpcKeys = new List<string>();
    private int maxNpcCount = 50;
    #endregion

    public void Init()
    {
        RegistItemPool();
        RegistNpcPool();
    }

    private void RegistItemPool()
    {
        for (int i = 0; i < mItemList.Count; i++)
        {
            for (int j = 0; j < maxItemCount; j++)
            {
                Items temp = Instantiate(mItemList[i]);
                Dictionary<int, Dictionary<string, List<Items>>> dTemp = null;
                Dictionary<string, List<Items>> ddTemp = null;
                List<Items> lTemp = null;

                if (mItemPool.TryGetValue(temp.Type, out dTemp))
                {
                    if(dTemp.TryGetValue(temp.Tier, out ddTemp))
                    {
                        if (ddTemp.TryGetValue(temp.ItemName, out lTemp))
                            lTemp.Add(temp);
                        else
                        {
                            lTemp = new List<Items>();
                            ddTemp.Add(temp.ItemName, lTemp);
                            lTemp.Add(temp);
                        }
                    }
                    else
                    {
                        ddTemp = new Dictionary<string, List<Items>>();
                        lTemp = new List<Items>();
                        ddTemp.Add(temp.ItemName, lTemp);
                        lTemp.Add(temp);
                        dTemp.Add(temp.Tier, ddTemp);
                    }
                }
                else
                {
                    dTemp = new Dictionary<int, Dictionary<string, List<Items>>>();
                    ddTemp = new Dictionary<string, List<Items>>();
                    lTemp = new List<Items>();
                    lTemp.Add(temp);
                    ddTemp.Add(temp.ItemName, lTemp);
                    dTemp.Add(temp.Tier, ddTemp);
                    mItemPool.Add(temp.Type, dTemp);
                }
                
            }
        }


        foreach(var type in mItemPool)
        {
            foreach(var tier in type.Value)
            {
                foreach(var name in tier.Value){
                    Dictionary<int, List<string>> dTemp = null;
                    List<string> lTemp = null;

                    if (mItemKey.TryGetValue(type.Key, out dTemp))
                    {
                        if(dTemp.TryGetValue(tier.Key, out lTemp))
                            lTemp.Add(name.Key);
                        else
                        {
                            lTemp = new List<string>();
                            lTemp.Add(name.Key);
                            dTemp.Add(tier.Key, lTemp);
                        }
                    }
                    else
                    {
                        dTemp = new Dictionary<int, List<string>>();
                        lTemp = new List<string>();
                        mItemKey.Add(type.Key, dTemp);
                        dTemp.Add(tier.Key, lTemp);
                        lTemp.Add(name.Key);
                    }
                }
            }
        }
    }
    
    private void RegistNpcPool()
    {
        for (int i = 0; i < mNpcInfo.Count; i++)
        {
            mNpcInfo[i].Init();
            for (int j = 0; j < maxNpcCount; j++)
            {
                GameObject temp = Instantiate(mNpcInfo[i].Prefab);
                temp.SetActive(false);
                NpcAI ai = temp.GetComponent<NpcAI>();
                ai.Init(mNpcInfo[i]);
                
                if (mNpcPool.ContainsKey(mNpcInfo[i].name))
                    mNpcPool[mNpcInfo[i].name].Add(temp);
                else
                {
                    List<GameObject> lTemp = new List<GameObject>();
                    mNpcPool.Add(mNpcInfo[i].name, lTemp);
                    mNpcKeys.Add(mNpcInfo[i].name);
                    lTemp.Add(temp);
                }
                    
            }
        }
    }

    // Get Item Func ==============================================
    public Items GetItem(string itemName) {

        foreach(var fKey in mItemPool)
        {
            foreach (var sKey in fKey.Value)
            {
                foreach(var tKey in sKey.Value)
                {
                    if(tKey.Key == itemName)
                    {
                        for (int i = 0; i < tKey.Value.Count; i++)
                        {
                            if (!tKey.Value[i].UnLock)
                            {
                                Debug.Log("해금안됨");
                                return null;
                            }
                            if (tKey.Value[i].IsUse) continue;
                            tKey.Value[i].IsUse = true;
                            return tKey.Value[i];
                        }
                    }
                }
            }
        }

        return null;
    }

    public Items GetRandomItem(Items.ITEM_TYPE type)
    {
        Items items = null;

        while (!items)
        {
            int max = 0;
            int rNum = 0;
            switch (type)
            {
                case Items.ITEM_TYPE.BASEMATERIAL:
                    max = Mathf.CeilToInt(Player.ShopLv / 5f) > 2 ? 2 : Mathf.CeilToInt(Player.ShopLv / 5f);
                    break;
                case Items.ITEM_TYPE.MATERIAL:
                    rNum = Random.Range(0, 3);
                    switch (rNum)
                    {
                        default:
                        case 0:
                            max = Player.ForgeLv;
                            break;
                        case 1:
                            max = Player.AlchmicLv;
                            break;
                        case 2:
                            max = Player.SewingLv;
                            break;
                    }

                    break;
                case Items.ITEM_TYPE.WEAPON:
                    max = Player.ForgeLv;
                    break;
                case Items.ITEM_TYPE.ARMOR:
                    rNum = Random.Range(0, 2);
                    switch (rNum)
                    {
                        default:
                        case 0:
                            max = Player.ForgeLv;
                            break;
                        case 1:
                            max = Player.SewingLv;
                            break;
                    }
                    break;
                case Items.ITEM_TYPE.TOOL:
                    max = Player.AlchmicLv;
                    break;
                default:
                    break;
            }
             
            int id = Random.Range(0, max);
            if (!mItemKey[type].ContainsKey(id)) break;

            string key = mItemKey[type][id][Random.Range(0, mItemKey[type][id].Count)];

            List<Items> temp = mItemPool[type][id][key];

            for (int i = 0; i < temp.Count; i++)
            {
                if (!temp[i].UnLock) break;
                if (temp[i].IsUse) continue;
                items = temp[i];
                break;
            }
        }

        if(items)
            items.IsUse = true;

        return items;
    }

    public Items GetRandomItem()
    {
        Items item = null;

        while (!item)
        {
            int min = (int)(Items.ITEM_TYPE.BASEMATERIAL);
            int max = (int)(Items.ITEM_TYPE.TOOL) + 1;
            Items.ITEM_TYPE type = (Items.ITEM_TYPE)(Random.Range(min, max));

            item = GetRandomItem(type);
        }

        return item;
    }

    public Dictionary<int, List<Items>> GetItems(Items.ITEM_TYPE type)
    {
        Dictionary<int, List<Items>> dTemp = new Dictionary<int, List<Items>>();

        foreach(var key in mItemPool[type])
        {
            foreach(var sKey in key.Value)
            {
                List<Items> lTemp = null;
                for (int i = 0; i < sKey.Value.Count; i++)
                {
                    if (sKey.Value[i].IsUse) continue;
                    Items temp = sKey.Value[i];
                    temp.IsUse = true;
                    if (dTemp.TryGetValue(key.Key, out lTemp))
                    {
                        lTemp.Add(temp);
                        break;
                    }
                    else
                    {
                        lTemp = new List<Items>();
                        dTemp.Add(key.Key, lTemp);
                        lTemp.Add(temp);
                        break;
                    }

                }
            }
        }

        return dTemp;
    }

    public Dictionary<int, List<Items>> GetItems(Items.ITEM_DETAILTYPE detailType)
    {
        Dictionary<int, List<Items>> dTemp = new Dictionary<int, List<Items>>();

        foreach(var fKey in mItemPool)
        {
            foreach (var sKey in mItemPool[fKey.Key])
            {
                foreach (var tKey in sKey.Value)
                {
                    List<Items> lTemp = null;
                    for (int i = 0; i < tKey.Value.Count; i++)
                    {
                        if (tKey.Value[i].DetailType != detailType) break;
                        if (tKey.Value[i].IsUse) continue;
                        Items temp = tKey.Value[i];
                        temp.IsUse = true;
                        if (dTemp.TryGetValue(sKey.Key, out lTemp))
                        {
                            lTemp.Add(temp);
                            break;
                        }
                        else
                        {
                            lTemp = new List<Items>();
                            dTemp.Add(sKey.Key, lTemp);
                            lTemp.Add(temp);
                            break;
                        }

                    }
                }
            }
        }
        

        return dTemp;
    }

    public Dictionary<int, List<Items>> GetItems(CraftMenu.CRAFT_TYPE type)
    {
        Dictionary<int, List<Items>> dTemp = new Dictionary<int, List<Items>>();

        foreach (var fKey in mItemPool)
        {
            foreach (var sKey in mItemPool[fKey.Key])
            {
                foreach (var tKey in sKey.Value)
                {
                    List<Items> lTemp = null;
                    for (int i = 0; i < tKey.Value.Count; i++)
                    {
                        if (tKey.Value[i].CraftPlace != type) break;
                        if (tKey.Value[i].IsUse) continue;
                        Items temp = tKey.Value[i];
                        temp.IsUse = true;
                        if (dTemp.TryGetValue(sKey.Key, out lTemp))
                        {
                            lTemp.Add(temp);
                            break;
                        }
                        else
                        {
                            lTemp = new List<Items>();
                            dTemp.Add(sKey.Key, lTemp);
                            lTemp.Add(temp);
                            break;
                        }

                    }
                }
            }
        }

        return dTemp;
    }

    public List<Items> GetTierItems(Items.ITEM_TYPE type, int tier)
    {
        List<Items> temp = new List<Items>();

        Dictionary<string, List<Items>> tDic = null;

        if (!mItemPool[type].ContainsKey(tier))
        {
            Debug.Log("해당 티어의 아이템이 없습니다.");
            return null;
        }
        
        foreach(KeyValuePair<string, List<Items>> item in tDic)
        {
            // 중복 체크
            bool isExist = false;
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].ItemName == item.Key)
                {
                    isExist = true;
                    break;
                }
            }
            if (isExist) continue;
        
        
            List<Items> tList = item.Value;
            
            for (int i = 0; i < tList.Count; i++)
            {
                if (tList[i].IsUse) continue;
                tList[i].IsUse = true;
                temp.Add(tList[i]);
                break;
            }
        }
    
        return temp;
    }

    public void UnLockItem(CraftMenu.CRAFT_TYPE type, int tier)
    {
        foreach(var fKey in mItemPool)
        {
            if (!fKey.Value.ContainsKey(tier)) return;

            foreach(var sKey in fKey.Value[tier])
            {
                if (sKey.Value.Count == 0) continue;
                if (sKey.Value[0].UnLock) continue;
                if (sKey.Value[0].CraftPlace != type) continue;

                for (int i = 0; i < sKey.Value.Count; i++)
                { 
                    sKey.Value[i].UnLock = true;
                }
            }
        }
    }

    //=============================================================

    // Get NPC Func ==============================================
    public GameObject GetNpc(string npcName)
    {
        List<GameObject> lTemp = mNpcPool[npcName];

        for (int i = 0; i < lTemp.Count; i++)
        {
            if (lTemp[i].activeSelf) continue;
            else
                return lTemp[i];
        }

        return null;
    }

    public GameObject GetRandomNpc()
    {
        int selectId = Random.Range(0, mNpcKeys.Count);

        return GetNpc(mNpcKeys[selectId]);
    }

    //=============================================================
}
