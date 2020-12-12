using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectItem : MonoBehaviour, IPointerDownHandler
{
    private CraftSlot target = null;
    private RectTransform tr;
    private Image itemImg;
    private RectTransform slotTr;
    private List<Items> itemList = null;
    private int id = 0;
    private Items selectItem = null;

    public delegate void SetItemDel(Items items, CraftSlot slot);
    public event SetItemDel SetCraftHandler;

    public void Init()
    {
        tr = GetComponent<RectTransform>();
        itemImg = transform.Find("Slot_exItem").GetChild(0).GetComponent<Image>();
        slotTr = itemImg.GetComponent<RectTransform>();
        ChangeImage(null);

        CraftSlot[] itemSlot = transform.parent.GetComponentsInChildren<CraftSlot>();
        for (int i = 0; i < itemSlot.Length; i++)
            itemSlot[i].showHandler += ShowWindow;
    }

    private void ShowWindow(CraftSlot _target, Vector2 offset)
    {
        id = 0;

        if(target == _target)
        {
            HideWindow();
        }
        else
        {
            target = _target;
            tr.position = target.transform.position + new Vector3(offset.x, offset.y, 0);
            itemList = target.GetItems();
            
            if (itemList == null)
            {
                selectItem = null;
                ChangeImage(null);
            }
            else
            {
                selectItem = itemList[id];
                ChangeImage(selectItem.Sprite);
            }

            gameObject.SetActive(true);
        }
    }

    public void HideWindow()
    {
        id = 0;
        target = null;
        itemList = null;
        selectItem = null;
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 mPos = eventData.pressPosition;
        Vector2 minRange = new Vector2(slotTr.position.x + slotTr.rect.xMin, slotTr.position.y + slotTr.rect.yMin);
        Vector2 maxRange = new Vector2(slotTr.position.x + slotTr.rect.xMax, slotTr.position.y + slotTr.rect.yMax);

        if (mPos.x < minRange.x || mPos.x > maxRange.x ||
            mPos.y < minRange.y || mPos.y > maxRange.y)
            return;
        
        SetCraftHandler(selectItem, target);

        HideWindow();
    }

    public void MoveNextItem()
    {
        if (itemList == null) return;
        id++;
        if (id >= itemList.Count)
            id = 0;
        selectItem = itemList[id];
        ChangeImage(selectItem.Sprite);
    }

    public void MovePrevItem()
    {
        if (itemList == null) return;
        id--;
        if (id < 0)
            id = itemList.Count - 1;

        selectItem = itemList[id];
        ChangeImage(selectItem.Sprite);
    }

    private void ChangeImage(Sprite sprite)
    {
        itemImg.sprite = sprite;

        if (!itemImg.sprite)
            SetAlpha(0f);
        else SetAlpha(1f);
    }

    private void SetAlpha(float a)
    {
        Color c = itemImg.color;
        c.a = a;
        itemImg.color = c;
    }
}
