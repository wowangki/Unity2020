using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    private List<string> mKeys = new List<string>();
    private Dictionary<string, MenuController> mMenuList = new Dictionary<string, MenuController>();

    private AudioSource mAudio;
    [SerializeField]
    private AudioClip mOpenAudioClip = null;
    [SerializeField]
    private AudioClip mCloseAudioClip = null;

    private void Awake()
    {
        mAudio = GetComponent<AudioSource>();

        for (int i = 0; i < transform.childCount; i++)
        {
            MenuController temp = transform.GetChild(i).GetComponent<MenuController>();

            if (!temp) continue;

            mKeys.Add(temp.name);
            mMenuList.Add(temp.name, temp);
            temp.Init();
            temp.mOpenAudioHandler += PlayOpenSound;
            temp.mCloseAudioHandler += PlayCloseSound;
        }
    }

    public void OpenMenu(string key)
    {
        mMenuList[key].OpenMenu();
    }

    public void CloseAllMenu()
    {
        for (int i = 0; i<mKeys.Count; i++)
        {
            mMenuList[mKeys[i]].CloseMenu();
        }
    }

    private void PlayOpenSound()
    {
        mAudio.clip = mOpenAudioClip;
        mAudio.Play();
    }

    private void PlayCloseSound()
    {
        mAudio.clip = mCloseAudioClip;
        mAudio.Play();
    }
}
