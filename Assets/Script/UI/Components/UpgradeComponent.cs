using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI DescText;

    [SerializeField]
    private TextMeshProUGUI UpgradeNameText;

    [SerializeField]
    private TextMeshProUGUI CostText;

    [SerializeField]
    private Button UpgradeBtn;

    private int UpgradeIdx = 0;

    private System.Numerics.BigInteger UpgradeCost;

    private UpgradeData UpgradeData;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Awake()
    {
        UpgradeBtn.onClick.AddListener(OnClickBtn);
    }

    public void Set(int upgradeidx)
    {
        UpgradeIdx = upgradeidx;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var upgradetd = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgradeidx));

        if(upgradetd != null)
        {
            UpgradeData = GameRoot.Instance.UserData.CurMode.UpgradeList.Find(x => x.UpgradeIdx == upgradeidx);

            UpgradeCost = upgradetd.cost;

            DescText.text = Tables.Instance.GetTable<Localize>().GetString(upgradetd.desc);

            UpgradeNameText.text = Tables.Instance.GetTable<Localize>().GetString(upgradetd.name);

            CostText.text = Utility.CalculateMoneyToString(UpgradeCost);


            GameRoot.Instance.UserData.CurMode.Money.Subscribe(x => {
                UpgradeBtn.interactable = x >= UpgradeCost;
            }).AddTo(disposables);
        }
    }

    public void OnClickBtn()
    {
        if(GameRoot.Instance.UserData.CurMode.Money.Value >= UpgradeCost)
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -UpgradeCost);

            UpgradeData.UpgradeGet();

            ProjectUtility.SetActiveCheck(this.gameObject, false);
        }

    }

}
