using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftStart : MonoBehaviour
{
    private Items.ItemRecipe[] mRecipe = null;
    private CraftResourceSlot[] mResource = null;
    private Image btnImage;
    private bool mIsProcess = false;

    public delegate IEnumerator DoProcessing(int value);
    public event DoProcessing ProcessHandler;

    private AudioSource mAudio;

    public void Init()
    {
        btnImage = GetComponent<Image>();
        SetColor(Color.red);

        CraftItemSlot _itemSlot = transform.parent.GetComponentInChildren<CraftItemSlot>();
        _itemSlot.resourceHandler += SetRecipe;

        mResource = transform.parent.GetComponentsInChildren<CraftResourceSlot>();

        for (int i = 0; i < mResource.Length; i++)
            mResource[i].AcceptHandler += ChkAccept;

        mAudio = GetComponent<AudioSource>();
    }

    public void ResetStart()
    {
        mRecipe = null;
        mIsProcess = false;
        SetColor(Color.red);
    }

    private void SetColor(Color c)
    {
        btnImage.color = c;
    }

    private void ChkAccept()
    {
        if (mRecipe == null)
        {
            SetColor(Color.red);
            return;
        }
        
        for (int i = 0; i < mRecipe.Length; i++)
        {
            Items.ITEM_DETAILTYPE[] nRecipe = mRecipe[i].Recipe;
            Items[] sRecipe = mRecipe[i].SpecialRecipe;

            bool _isCorrect = true;
            if (nRecipe == null) _isCorrect = false;
            else
            {
                for (int j = 0; j < nRecipe.Length; j++)
                {
                    if (nRecipe[j] == Items.ITEM_DETAILTYPE.GEM) continue;
                    if (!mResource[j].Item || mResource[j].Item.DetailType != nRecipe[j])
                    {
                        _isCorrect = false;
                        break;
                    }
                }
            }

            if (!_isCorrect || sRecipe != null)
            {
                for (int j = 0; j < sRecipe.Length; j++)
                {
                    if (!mResource[j].Item || sRecipe[j].ItemName != mResource[j].Item.ItemName)
                    {
                        _isCorrect = false;
                        break;
                    }
                }
            }

            mIsProcess = _isCorrect;
            if (_isCorrect)
                break;
        }

        if (mIsProcess)
            SetColor(Color.green);
        else
            SetColor(Color.red);
    }
    
    private void SetRecipe(Items item)
    {
        if (!item) return;
        mRecipe = item.Recipe;
        ChkAccept();
    }

    public void DoProcess()
    {
        if (!mIsProcess) return;

        int value = 0;

        for (int i = 0; i < mResource.Length; i++)
        {
            if (!mResource[i].gameObject.activeSelf) continue;
            if (!mResource[i].Item) continue;

            value += mResource[i].Item.Value;
            Inventory.UseItem(mResource[i].Item);
        }

        mAudio.Play();

        GameManager.Instance.BackGroundCoroutine(ProcessHandler(value));
    }
}
