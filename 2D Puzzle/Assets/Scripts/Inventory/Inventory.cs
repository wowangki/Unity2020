using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MenuController
{
    [SerializeField]
    private GameObject[] mSlotList = null;
    private GameObject curSlot = null;
    static public List<List<InvenSlot>> Slots { get; } = new List<List<InvenSlot>>();

    public override void Init()
    {
        base.Init();
        RegistInventory();
        curSlot = mSlotList[0];
    }

    private void RegistInventory()
    {
        for (int i = 0; i < mSlotList.Length; i++)
        {
            RectTransform tr = mSlotList[i].GetComponent<RectTransform>();
            GridLayoutGroup grid = mSlotList[i].GetComponent<GridLayoutGroup>();
            Vector2 cellSize = grid.cellSize;
            Vector2 spacing = grid.spacing;

            InvenSlot[] slots = mSlotList[i].GetComponentsInChildren<InvenSlot>();

            int tX = (int)Mathf.Round((tr.sizeDelta.x / (cellSize.x + spacing.x)));
            int tY = (int)Mathf.Round((tr.sizeDelta.y / (cellSize.y + spacing.y)));

            for (int y = 0; y < tY; y++)
            {
                List<InvenSlot> lTemp = new List<InvenSlot>();
                for (int x = 0; x < tX; x++)
                {
                    InvenSlot temp = slots[(y * tX) + x];
                    temp.Init();
                    lTemp.Add(temp);
                }
                Slots.Add(lTemp);
            }
        }
    }

    public void ChangeSlotList(int num)
    {
        if (num >= mSlotList.Length) return;

        curSlot.gameObject.SetActive(false);

        curSlot = mSlotList[num];
        curSlot.gameObject.SetActive(true);
    }

    static public void StoreItem(Items _item)
    {
        for (int y = 0; y < Slots.Count; y++)
        {
            for (int x = 0; x < Slots[y].Count; x++)
            {
                InvenSlot temp = Slots[y][x];

                if (!temp.Item)
                {
                    temp.AddItem(_item);
                    return;
                }
                else if (temp.Item.ItemName == _item.ItemName && temp.MaxCount > temp.Count)
                {
                    temp.AddItem(_item);
                    return;
                }
            }
        }
    }

    static public bool UseItem(Items _item)
    {
        for (int y = Slots.Count - 1; y >= 0; y--)
        {
            for (int x = Slots[y].Count - 1; x >= 0; x--)
            {
                InvenSlot temp = Slots[y][x];
                if (!temp.Item) continue;

                if (temp.Item.ItemName == _item.ItemName)
                {
                    temp.UseItem();
                    return true;
                }
            }
        }

        return false;
    }
    
    static public InvenSlot GetInvenSlot(Items item)
    {
        InvenSlot slots = null;

        for (int y = Slots.Count - 1; y >= 0; y--)
        {
            for (int x = Slots[y].Count - 1; x >= 0; x--)
            {
                if (!Slots[y][x].Item) continue;
                if (Slots[y][x].Item.ItemName != item.ItemName) continue;
                slots = Slots[y][x];
                break;
            }
        }

        return slots;
    }

    static public List<InvenSlot> GetInvenSlots(Items.ITEM_TYPE type)
    {
        List<InvenSlot> temp = new List<InvenSlot>();

        for (int y = 0; y < Slots.Count; y++)
        {
            for (int x = 0; x < Slots[y].Count; x++)
            {
                if (!Slots[y][x].Item) continue;
                if (Slots[y][x].Item.Type != type) continue;

                temp.Add(Slots[y][x]);
            }
        }

        if (temp.Count == 0)
            temp = null;
        
        return temp;
    }

    static public Items GetInvenItem(Items.ITEM_TYPE type)
    {
        Items temp = null;

        for (int y = 0; y < Slots.Count; y++)
        {
            for (int x = 0; x < Slots[x].Count; x++)
            {
                if (!Slots[y][x].Item) continue;
                if (Slots[y][x].Item.Type != type) continue;
                return temp = GameManager.Instance.ObjPool.GetItem(Slots[y][x].Item.ItemName);
            }
        }

        return temp;
    }

    static public List<Items> GetInvenItemList(Items.ITEM_DETAILTYPE type)
    {
        List<Items> temp = new List<Items>();

        for (int y = 0; y < Slots.Count; y++)
        {
            for (int x = 0; x < Slots[y].Count; x++)
            {
                if (!Slots[y][x].Item) continue;
                Items tItem = Slots[y][x].Item;
                if (tItem.DetailType != type) continue;

                bool isExist = false;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (tItem.ItemName == temp[i].ItemName)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist) continue;
                temp.Add(GameManager.Instance.ObjPool.GetItem(tItem.ItemName));
            }
        }

        if (temp.Count == 0)
            return null;

        return temp;
    }
}
