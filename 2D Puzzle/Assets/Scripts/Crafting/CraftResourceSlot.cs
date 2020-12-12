using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftResourceSlot : CraftSlot
{
    private int slotId = 0;

    public delegate void AcceptDel();
    public event AcceptDel AcceptHandler;

    public void Init(int id)
    {
        base.Init();
        slotId = id;
    }

    public override void SetSlot(Items items, CraftSlot slot)
    {
        if (slot != this) return;
        base.SetSlot(items, slot);
        AcceptHandler();
    }

    public override List<Items> GetItems()
    {
        Items mCraftItem = GetComponentInParent<CraftLine>().CurItem;
        if (!mCraftItem) return null;
        List<Items> tList = new List<Items>();
        Items.ItemRecipe[] recipe = mCraftItem.Recipe;

        for (int i = 0; i < recipe.Length; i++)
        {
            List<Items> temp = new List<Items>();
            if (recipe[i].Recipe.Length > slotId)
            {
                Items.ITEM_DETAILTYPE type = recipe[i].Recipe[slotId];
                List<Items> ttemp = Inventory.GetInvenItemList(type);

                if(ttemp != null)
                    temp.AddRange(ttemp);
            }
                 
            if (recipe[i].SpecialRecipe.Length > slotId)
            {
                InvenSlot tSlot = Inventory.GetInvenSlot(recipe[i].SpecialRecipe[slotId]);
                if (tSlot)
                    temp.Add(tSlot.Item);
            }
             

            if(temp.Count != 0)
                tList.AddRange(temp);
        }

        if (tList.Count == 0)
            return null;

        return tList;
    }
}
