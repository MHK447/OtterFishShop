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
    private TextMeshProUGUI RewardValueText;


    [SerializeField]
    private Slider TimeSliderValue;

    [SerializeField]
    private TextMeshProUGUI MaxTimeText;

    [SerializeField]
    private TextMeshProUGUI CurTimeText;

    [SerializeField]
    private TextMeshProUGUI UpBenefitText;

    [SerializeField]
    private TextMeshProUGUI MiddleBenefitText;

    [SerializeField]
    private Button RewardBtn;


    [SerializeField]
    private Button ADRewardBtn;

    private int TiemSecond = 0;

    private System.Numerics.BigInteger RewardValue = 0 ;


    protected override void Awake()
    {
        base.Awake();

        ADRewardBtn.onClick.AddListener(OnClickAdReward);

        RewardBtn.onClick.AddListener(OnClickReward);
    }
    public void Set(int timesecond)
    {
        TiemSecond = timesecond;

        RewardValue = ProjectUtility.CalcOfflineReward(timesecond);

        RewardValueText.text = ProjectUtility.CalculateMoneyToString(RewardValue);

        AdRewardValueText.text =
         ProjectUtility.CalculateMoneyToString(RewardValue * GameRoot.Instance.InGameSystem.offline_reward_multiple);
    }

    public void OnClickAdReward()
    {
        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency , (int)Config.CurrencyID.Money , RewardValue *
        GameRoot.Instance.InGameSystem.offline_reward_multiple);
        Hide();
    }

    public void OnClickReward()
    {
        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency , (int)Config.CurrencyID.Money , RewardValue);
        Hide();
    }
}
