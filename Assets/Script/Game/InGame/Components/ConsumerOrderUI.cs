using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

[UIPath("UI/InGame/ConsumerOrderUI", false)]
public class ConsumerOrderUI : InGameFloatingUI
{
    public enum ConsumerState
    {
        Food = 0,
        Counter,
        Pay,
    }

    [SerializeField]
    private Image OrderImg;

    [SerializeField]
    private Text CountText;

    [SerializeField]
    private Image SliderValue;

    [SerializeField]
    private List<GameObject> ConsumerStateList = new List<GameObject>();

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

        SetImage(ConsumerOrderUI.ConsumerState.Food);
    }

    public void SetFacilityImg(int facilityidx)
    {

        if (facilityidx > 0 && facilityidx < 100) //기본 물품대 
        {
            SetImage(ConsumerOrderUI.ConsumerState.Food);
        }
        else if (facilityidx > 99 && facilityidx < 1000) // 조리대 
        {

        }
        else if (facilityidx == 1000) //계산대
        {
            SetImage(ConsumerOrderUI.ConsumerState.Counter);
        }
    }



    public void SetImage(ConsumerState state)
    {
        foreach(var obj in ConsumerStateList)
        {
            ProjectUtility.SetActiveCheck(obj, false);
        }

        ProjectUtility.SetActiveCheck(ConsumerStateList[(int)state], true);
    }


    public void SetCountText(int count)
    {
        SliderValue.fillAmount = (float)count / (float)MaxCount;
        CountText.text = $"{count}/{MaxCount}";
    }

    
}
