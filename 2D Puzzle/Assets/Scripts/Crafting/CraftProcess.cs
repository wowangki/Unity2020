using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CraftProcess : MonoBehaviour, IPointerDownHandler
{
    private Image mCoverImg;
    private Image mProcessBar;
    private Text text;
    private IEnumerator textCor = null;
    private Items compeleteItem = null;

    public delegate void ResetProcessDel();
    public ResetProcessDel ResetHandler;

    public void Init()
    {
        mCoverImg = GetComponent<Image>();
        mProcessBar = transform.Find("Cover_Fill").GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        ResetProcess();
        ResetHandler += ResetProcess;

        transform.parent.GetComponentInChildren<CraftStart>().ProcessHandler += ProcessCoroutine;
    }

    private IEnumerator ProcessCoroutine(int value)
    {
        Items temp = transform.parent.GetComponent<CraftLine>().CurItem;

        compeleteItem = GameManager.Instance.ObjPool.GetItem(temp.ItemName);
        compeleteItem.Value = value;

        float coverSpeed = 0.1f;

        while (mCoverImg.fillAmount < 1f)
        {
            mCoverImg.fillAmount += coverSpeed;
            coverSpeed += 0.1f;

            if (mCoverImg.fillAmount > 1f)
                mCoverImg.fillAmount = 1f;

            yield return null;

            if (coverSpeed > 1f)
                coverSpeed = 1f;
        }

        if (compeleteItem.Value == 0)
            compeleteItem.Value = 1;

        coverSpeed = (float)(1f / (value * 2f));
        text.text = "CRAFTING";
        textCor = ProcessTextCoroutine();
        StartCoroutine(textCor);

        while(mProcessBar.fillAmount < 1f)
        {
            mProcessBar.fillAmount += coverSpeed * 0.25f;
            yield return null;

            if (mProcessBar.fillAmount < 0.3f)
                SetColor(Color.red, mProcessBar);
            else if (mProcessBar.fillAmount < 0.7f)
                SetColor(Color.yellow, mProcessBar);
            else
                SetColor(Color.green, mProcessBar);

            if (mProcessBar.fillAmount > 1f)
                mProcessBar.fillAmount = 1f;
        }

        StopCoroutine(textCor);
        text.text = "Compelete!!!";
        SetColor(Color.yellow, text);
        mProcessBar.raycastTarget = true;
        mProcessBar.maskable = true;

        yield return null;
    }

    private IEnumerator ProcessTextCoroutine()
    {
        WaitForSeconds ws = new WaitForSeconds(1f);
        text.text += ".";

        if (text.text.Contains("..."))
            text.text = "CRAFTING";

        yield return ws;
    }

    private void ResetProcess()
    {
        mCoverImg.fillAmount = 0f;
        mProcessBar.fillAmount = 0f;
        text.text = "";
        SetColor(Color.red, mProcessBar);
        SetColor(Color.black, text);
        mProcessBar.raycastTarget = false;
        mProcessBar.maskable = false;
    }

    private void CompeleteProcess()
    {
        Inventory.StoreItem(compeleteItem);
        compeleteItem = null;

        ResetHandler();
    }

    private void SetColor(Color _c, Image img)
    {
        img.color = _c;
    }

    private void SetColor(Color _c, Text txt)
    {
        txt.color = _c;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mProcessBar.fillAmount < 1f) return;

        CompeleteProcess();
    }
}
