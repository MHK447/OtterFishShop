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



    public void Set(int rewardtype , int rewardidx , int rewardcount)
    {
        MoneyText.text = rewardcount.ToString();
        
    }

    public void SliderValue(int rewardcount , int goalcount)
    {
        SliderImg.fillAmount = (float)rewardcount / (float)goalcount;
    }

}
