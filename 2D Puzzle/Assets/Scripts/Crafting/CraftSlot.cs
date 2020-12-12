using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftSlot : MonoBehaviour, IPointerDownHandler
{
    protected Image itemImg;
    public Items Item { get; protected set; }
    [SerializeField]
    protected Vector2 offset = Vector2.zero;

    public delegate void ShowSelectDel(CraftSlot slot, Vector2 offset);
    public event ShowSelectDel showHandler;

    private AudioSource mAudio;

    virtual public void Init()
    {
        itemImg = transform.GetChild(0).GetComponent<Image>();
        mAudio = GetComponent<AudioSource>();
    }

    public virtual void ResetSlot()
    {
        Item = null;
        itemImg.sprite = null;
        SetAlpha(0f);
    }

    public void ChangeImage(Sprite sprite)
    {
        if (!sprite)
        {
            SetAlpha(0f);
            return;
        }

        itemImg.sprite = sprite;
        SetAlpha(1f);
    }

    protected void SetAlpha(float a)
    {
        Color c = itemImg.color;
        c.a = a;
        itemImg.color = c;
    }

    public virtual List<Items> GetItems() { return null; }

    public virtual void SetSlot(Items items, CraftSlot slot)
    {
        if (slot != this) return;

        Item = items;
        if (Item)
            ChangeImage(Item.Sprite);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (showHandler != null)
            showHandler(this, offset);

        if (mAudio)
            mAudio.Play();
    }
}
