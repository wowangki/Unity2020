using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GaugeBar
{
    public enum GAUGE_TYPE
    {
        HP,
        ARMOR,
        O2
    }

    public GameObject myObject;
    public GAUGE_TYPE type;
    public Image curGauge;
    public Image delayGauge;

    public int curValue;
    public int delayValue;
    public int maxValue;
}


public class HUD_Status : MonoBehaviour
{
    private Status status;
    [SerializeField] private List<GaugeBar> gaugeList = new List<GaugeBar>();

    public void InitGauge(Status status)
    {
        this.status = status;

        for (int i = 0; i < gaugeList.Count; i++)
        {
            switch ((GaugeBar.GAUGE_TYPE)i)
            {
                case GaugeBar.GAUGE_TYPE.HP:
                    gaugeList[i].curValue = status.Hp;
                    gaugeList[i].delayValue = status.Hp;
                    gaugeList[i].maxValue = status.MaxHp;
                    break;
                case GaugeBar.GAUGE_TYPE.ARMOR:
                    gaugeList[i].curValue = status.Armor;
                    gaugeList[i].delayValue = status.Armor;
                    gaugeList[i].maxValue = status.MaxArmor;
                    break;
                case GaugeBar.GAUGE_TYPE.O2:
                    gaugeList[i].curValue = status.Oxygen;
                    gaugeList[i].delayValue = status.Oxygen;
                    gaugeList[i].maxValue = status.MaxOxygen;
                    break;
            }

            if (gaugeList[i].maxValue <= 0) continue;

            gaugeList[i].curGauge.fillAmount = ((float)gaugeList[i].curValue / gaugeList[i].maxValue);
            gaugeList[i].delayGauge.fillAmount = ((float)gaugeList[i].delayValue / gaugeList[i].maxValue);
        }
    }

    private void Update()
    {
        if (!status) return;

        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        for (int i = 0; i < gaugeList.Count; i++)
        {
            switch ((GaugeBar.GAUGE_TYPE)i)
            {
                case GaugeBar.GAUGE_TYPE.HP:
                    gaugeList[i].curValue = status.Hp;
                    gaugeList[i].maxValue = status.MaxHp;
                    break;
                case GaugeBar.GAUGE_TYPE.ARMOR:
                    gaugeList[i].curValue = status.Armor;
                    gaugeList[i].maxValue = status.MaxArmor;

                    if (gaugeList[i].maxValue > 0)
                        gaugeList[i].myObject.SetActive(true);
                    else
                        gaugeList[i].myObject.SetActive(false);

                    break;
                case GaugeBar.GAUGE_TYPE.O2:
                    gaugeList[i].curValue = status.Oxygen;
                    gaugeList[i].maxValue = status.MaxOxygen;

                    if (gaugeList[i].curValue < gaugeList[i].maxValue)
                        gaugeList[i].myObject.SetActive(true);
                    else
                        gaugeList[i].myObject.SetActive(false);

                    break;
            }

            gaugeList[i].curGauge.fillAmount = ((float)gaugeList[i].curValue / gaugeList[i].maxValue);

            if (gaugeList[i].curValue != gaugeList[i].delayValue)
                StartCoroutine(DelayGaugeCoroutine(gaugeList[i]));
        }
    }

    private IEnumerator DelayGaugeCoroutine(GaugeBar gauge)
    {
        WaitForSeconds ws = new WaitForSeconds(0.5f);

        while (true)
        {
            if (gauge.curValue > gauge.delayValue)
                gauge.delayValue++;
            else if (gauge.curValue < gauge.delayValue)
                gauge.delayValue--;
            else yield break;

            gauge.delayGauge.fillAmount = (float)gauge.delayValue / gauge.maxValue;
            yield return ws;

        }
    }
}
