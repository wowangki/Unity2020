using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InvenSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Items Item { get; private set; } = null;
    private Image mItemImg;
    private Text mText;
    public int Count { get; private set; } = 0;
    public int MaxCount { get; private set; } = 0;

    public void Init()
    {
        mItemImg = transform.Find("Item_Img").GetComponent<Image>();
        mText = transform.Find("Item_Count").GetComponent<Text>();
        mText.text = "";
        SetAlpha(0f);
    }

    public void AddItem(Items _item)
    {
        if (!Item)
        {
            Item = _item;
            mItemImg.sprite = Item.Sprite;
            MaxCount = Item.Kind == Items.ITEM_KIND.RESOURCE ? 99 : 1;
            SetAlpha(1f);
        }

        Count++;
        if (Count <= 1)
            mText.text = "";
        else 
            mText.text = Count.ToString();
    }

    public void UseItem()
    {
        Count--;
        if (Count <= 0)
            ClearSlot();
        else
            mText.text = Count.ToString();
    }

    public void ClearSlot()
    {
        Item.IsUse = false;
        Item = null;
        Count = 0;
        MaxCount = 0;
        mItemImg.sprite = null;
        mText.text = "";
        SetAlpha(0f);
    }

    public void SetAlpha(float alpha)
    {
        Color c = mItemImg.color;
        c.a = alpha;
        mItemImg.color = c;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!Item) return;
        DragSlot.instance.mDragSlot = this;
        DragSlot.instance.DragSetImage(mItemImg);
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!Item) return;
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0f);
        DragSlot.instance.mDragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.mDragSlot)
            SwapSlot();
    }

    private void SwapSlot()
    {
        Items temp = Item;
        int tCount = Count;

        InvenSlot _dragging = DragSlot.instance.mDragSlot;

        ChangeSlot(_dragging);

        if (!temp)
            _dragging.ClearSlot();
        else
            _dragging.ChangeSlot(temp, tCount);
    }

    private void ChangeSlot(InvenSlot _invenSlot)
    {
        if (!_invenSlot.Item)
        {
            SetAlpha(0f);
            return;
        }

        Item = _invenSlot.Item;
        mItemImg.sprite = Item.Sprite;
        Count = _invenSlot.Count;
        MaxCount = Item.Kind == Items.ITEM_KIND.RESOURCE ? 99 : 1;
        SetAlpha(1f);
    }

    private void ChangeSlot(Items _item, int _count)
    {
        Item = _item;
        mItemImg.sprite = Item.Sprite;
        Count = _count;
        MaxCount = Item.Kind == Items.ITEM_KIND.RESOURCE ? 99 : 1;
        SetAlpha(1f);
    }
}
