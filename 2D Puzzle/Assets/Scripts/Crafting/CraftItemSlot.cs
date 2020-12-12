using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftItemSlot : CraftSlot
{
    private CraftMenu menu;
    public delegate void ActivatingResourceSlotDel(Items items);
    public event ActivatingResourceSlotDel resourceHandler;

    public override void Init()
    {
        base.Init();
    }

    public override List<Items> GetItems()
    {
        if (!menu)
            menu = GetComponentInParent<CraftMenu>();

        int lv = 0;
        switch (menu.Type)
        {
            case CraftMenu.CRAFT_TYPE.FORGE:
                lv = Player.ForgeLv;
                break;
            case CraftMenu.CRAFT_TYPE.ALCHMIC:
                lv = Player.AlchmicLv;
                break;
            case CraftMenu.CRAFT_TYPE.SEWING:
                lv = Player.SewingLv;
                break;
        }

        List<Items> temp = new List<Items>();

        for (int i = 0; i <= lv; i++)
        {
            if (!menu.CraftingList.ContainsKey(i)) continue;
            temp.AddRange(menu.CraftingList[i]);
        }

        if (temp.Count == 0)
            temp = null;

        return temp;
    }

    public override void SetSlot(Items items, CraftSlot slot)
    {
        if (slot != this) return;
        base.SetSlot(items, slot);
        resourceHandler(Item);
    }
    
}
