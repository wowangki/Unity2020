using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Gold : MonoBehaviour
{
    private Text mText;

    private void Awake()
    {
        mText = GetComponentInChildren<Text>();
        mText.text = "0";

        Player player = GameObject.Find("Player").GetComponent<Player>();

        if (player)
            player.GoldUIHandler += ShowGold;
    }

    private void ShowGold(int value)
    {
        mText.text = value.ToString();
    }
}
