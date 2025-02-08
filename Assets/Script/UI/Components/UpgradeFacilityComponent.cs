using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using TMPro;



public class UpgradeFacilityComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI BefroreValueText;

    [SerializeField]
    private TextMeshProUGUI AfterValueText;

    [SerializeField]
    private TextMeshProUGUI CurCostValueText;

    [SerializeField]
    private TextMeshProUGUI LevelText;

    [SerializeField]
    private ButtonPressed UpgradeBtn;

    private int FacilityIdx = 0;

    private StageFishUpgradeData CurStageFacilityData = null;

    private System.Numerics.BigInteger CurPrice = 0;

    private FacilityUpgradeData FacilityUpgradeData;

    private void Awake()
    {
        UpgradeBtn.OnPressed = () => OnClickUpgrade();
    }


    public void Set(int facilityidx)
    {
        FacilityIdx = facilityidx;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, FacilityIdx));

        FacilityUpgradeData = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, facilityidx));

        if (td != null)
        {
            CurStageFacilityData = GameRoot.Instance.FacilitySystem.GetFacilityUpgradeData(FacilityIdx);
            SetInfo();

        }
    }


    public void SetInfo()
    {
        LevelText.text = Tables.Instance.GetTable<Localize>().GetFormat("str_level", CurStageFacilityData.Level);

        CurPrice = GameRoot.Instance.FacilitySystem.GetFishUpgradeLevelCost(FacilityIdx, CurStageFacilityData.Level);


        CurCostValueText.text = Utility.CalculateMoneyToString(CurPrice);

        BefroreValueText.text = Utility.CalculateMoneyToString(GameRoot.Instance.FacilitySystem.GetFishCurSellProductValue(FacilityIdx, CurStageFacilityData.Level));
        AfterValueText.text = Utility.CalculateMoneyToString(GameRoot.Instance.FacilitySystem.GetFishCurSellProductValue(FacilityIdx, CurStageFacilityData.Level + 1));

    }

    public void OnClickUpgrade()
    {
        if(CurPrice <= GameRoot.Instance.UserData.CurMode.Money.Value)
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency ,(int)Config.CurrencyID.Money,  -CurPrice);

            CurStageFacilityData.Level += 1;

            SetInfo();
        }

    }
}
