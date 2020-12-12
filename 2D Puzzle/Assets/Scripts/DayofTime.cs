using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayofTime : MonoBehaviour
{
    private Image mImage;

    [SerializeField]
    private List<Sprite> timeImgeList = new List<Sprite>();
    private int mImgIndex = 0;
    [SerializeField]
    private float mCycleTime = 30.0f;

    [SerializeField]
    private bool mIsOpen = false;

    private void Awake()
    {
        mImage = GetComponent<Image>();
    }

    private void Start()
    {
        StartCoroutine(CycleCoroutine());    
    }

    IEnumerator CycleCoroutine()
    {
        WaitForSeconds ws = new WaitForSeconds(mCycleTime);

        while (mIsOpen)
        {
            mImage.sprite = timeImgeList[++mImgIndex];
            yield return ws;

            if (mImgIndex >= timeImgeList.Count - 1)
                mImgIndex = 0;
        }
    }
}
