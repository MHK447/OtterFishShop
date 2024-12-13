using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

[UIPath("UI/InGame/ConsumerOrderUI", false)]
public class ConsumerOrderUI : InGameFloatingUI
{
    [SerializeField]
    private Image OrderImg;

    [SerializeField]
    private Text CountText;

    [SerializeField]
    private Image SliderValue;

    private Consumer Consumer;

    private int MissionIdx = 0;

    private int MaxCount = 0; 

    public void Set(Consumer targetconsumer , int missionidx , int count  , int maxcount)
    {
        Consumer = targetconsumer;

        MissionIdx = missionidx;

        MaxCount = maxcount;

        CountText.text = $"{count}/{maxcount}";

        SliderValue.fillAmount = 0f;
    }


    public void SetImage(int missionidx)
    {

    }


    public void SetCountText(int count)
    {
        SliderValue.fillAmount = (float)count / (float)MaxCount;
        CountText.text = $"{count}/{MaxCount}";
    }

    
}
