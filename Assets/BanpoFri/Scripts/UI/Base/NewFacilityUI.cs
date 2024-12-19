using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

[UIPath("UI/InGame/NewFacilityUI", false)]
public class NewFacilityUI : InGameFloatingUI
{
    [SerializeField]
    private Text MoneyText;

    [SerializeField]
    private Image RewardImg;

    [SerializeField]
    private Image SliderImg;


    public void SliderValue(int rewardcount , int goalcount)
    {
        MoneyText.text = goalcount.ToString();
        SliderImg.fillAmount = (float)rewardcount / (float)goalcount;
    }

}
