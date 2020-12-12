using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    protected GameObject menuList = null;

    public delegate void PlayOpenMenu();
    public event PlayOpenMenu mOpenAudioHandler;
    public delegate void PlayCloseMenu();
    public event PlayCloseMenu mCloseAudioHandler;

    public virtual void Init()
    {
        menuList = transform.parent.Find("MenuList").gameObject;
    }

    public virtual void OpenMenu()
    {
        gameObject.SetActive(true);
        menuList.SetActive(false);
        mOpenAudioHandler();
    }

    public virtual void CloseMenu()
    {
        if (gameObject.activeSelf)
            mCloseAudioHandler();

        menuList.SetActive(true);
        gameObject.SetActive(false);
    }
}
