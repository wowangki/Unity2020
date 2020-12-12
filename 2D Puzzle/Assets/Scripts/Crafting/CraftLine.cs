using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CraftLine : MonoBehaviour
{
    public Items CurItem { get; private set; } = null;
    private CraftItemSlot mCraftItem;
    private List<CraftResourceSlot> mResource;
    private CraftStart mAccept;
    private CraftProcess mProcess;

    public void Init()
    {
        mCraftItem = GetComponentInChildren<CraftItemSlot>();
        mResource = GetComponentsInChildren<CraftResourceSlot>().ToList();
        mAccept = GetComponentInChildren<CraftStart>();
        mProcess = transform.Find("Crafting_Cover").GetComponentInChildren<CraftProcess>();

        mCraftItem.Init();
        mCraftItem.resourceHandler += SetCurItem;
        mProcess.ResetHandler += mCraftItem.ResetSlot;

        for (int i = 0; i < mResource.Count;i++)
        {
            mResource[i].Init(i);
            mProcess.ResetHandler += mResource[i].ResetSlot;
        }
        mAccept.Init();
        mProcess.ResetHandler += mAccept.ResetStart;
        mProcess.Init();

        SelectItem select = transform.parent.parent.Find("SelectWindow").GetComponent<SelectItem>();
        select.SetCraftHandler += mCraftItem.SetSlot;
        
        for (int i = 0; i < mResource.Count; i++)
            select.SetCraftHandler += mResource[i].SetSlot;

        mProcess.ResetHandler += select.HideWindow;
    }
    
    private void SetCurItem(Items _item) {

        CurItem = _item;
        if (!CurItem) return;

        for (int i = 0; i < mResource.Count; i++)
            mResource[i].gameObject.SetActive(false);

        int slotCount = 0;

        for (int i = 0; i < CurItem.Recipe.Length; i++)
        {
            int _Count = 0;
            int nRCount = 0;
            int nSCount = 0;
            if (CurItem.Recipe[i].Recipe != null) nRCount = CurItem.Recipe[i].SpecialRecipe.Length;
            if (CurItem.Recipe[i].SpecialRecipe != null) nSCount = CurItem.Recipe[i].Recipe.Length;

            _Count = nRCount >= nSCount ? nRCount : nSCount;

            if (slotCount < _Count)
                slotCount = _Count;
        }

        for (int i = 0; i < slotCount; i++)
        {
            mResource[i].gameObject.SetActive(true);
        }
    }
}
