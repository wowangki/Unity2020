using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleBlock : MonoBehaviour, IPointerClickHandler
{
    private Image blockImg;
    public Image itemImg;
    public PuzzleRoad Road { get; set; } = null;
    public Items Item { get; set; } = null;
    private AudioSource mAudio;

    public void Init()
    {
        Image[] img = GetComponentsInChildren<Image>();
        blockImg = img[0];
        itemImg = img[1];

        mAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (Item)
            itemImg.sprite = Item.Sprite;
    }

    public void Select()
    {
        Road.node.IsClose = false;
        SetAlpha(blockImg, 0.5f);
        SetAlpha(itemImg, 0.5f);
    }

    public void UnSelect()
    {
        Road.node.IsClose = true;
        SetAlpha(blockImg, 1f);
        SetAlpha(itemImg, 1f);
    }

    public void BreakBlock()
    {
        SetAlpha(blockImg, 1f);
        SetAlpha(itemImg, 1f);
        GameManager.Instance.Spawn.RegistSpawnBlock(this);
        Inventory.StoreItem(Item);
        // 셔플 블록 제거

        gameObject.SetActive(false);
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Puzzle.Start) Puzzle.Start = Road;
        else if (!Puzzle.End) Puzzle.End = Road;

        Select();
        mAudio.Play();
    }
}
