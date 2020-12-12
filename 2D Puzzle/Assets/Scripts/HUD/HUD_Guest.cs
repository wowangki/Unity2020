using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Guest : MonoBehaviour
{
    static private List<HUD_GuestCondition> mConditions = new List<HUD_GuestCondition>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            HUD_GuestCondition temp = transform.GetChild(i).GetComponent<HUD_GuestCondition>();
            if (temp)
            {
                mConditions.Add(temp);
                temp.Init();
            }
        }
    }

    static public HUD_GuestCondition GetUI(NpcAI ai)
    {
        HUD_GuestCondition temp = null;

        for (int i = 0; i < mConditions.Count; i++)
        {
            if (mConditions[i].gameObject.activeSelf) continue;

            temp = mConditions[i];
            temp.transform.position = Camera.main.WorldToScreenPoint(ai.transform.position);
            temp.TargetInfo = ai.Info;
            temp.gameObject.SetActive(true);
            break;
        }

        return temp;
    }
}
