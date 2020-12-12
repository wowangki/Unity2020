using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public InvenSlot mDragSlot;

    private Image imgItem;

    private void Awake()
    {
        instance = this;
        imgItem = GetComponent<Image>();
    }

    public void DragSetImage(Image _itemImg)
    {
        imgItem.sprite = _itemImg.sprite;
        SetColor(1);
    }

    public void SetColor(float alpha)
    {
        Color color = imgItem.color;
        color.a = alpha;
        imgItem.color = color;
    }
}
