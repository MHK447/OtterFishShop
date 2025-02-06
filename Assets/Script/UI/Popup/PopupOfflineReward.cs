using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;

[UIPath("UI/Popup/PopupOfflineReward")]
public class PopupOfflineReward : UIBase
{
    [SerializeField]
    private TextMeshProUGUI AdRewardValueText;

    [SerializeField]
    private Slider TimeSliderValue;

    [SerializeField]
    private TextMeshProUGUI MaxTimeText;

    [SerializeField]
    private TextMeshProUGUI CurTimeText;


    [SerializeField]
    private TextMeshProUGUI CurRewardValueText;

    [SerializeField]
    private Button ConfrimBtn;


    [SerializeField]
    private Button ADRewardBtn;

    [SerializeField]
    private TextMeshProUGUI ValueText;

    [SerializeField]
    private TextMeshProUGUI RewardValueText;

    private int TiemSecond = 0;
    public void Set(int timesecond)
    {
        TiemSecond = timesecond;

        
        
    }



}
