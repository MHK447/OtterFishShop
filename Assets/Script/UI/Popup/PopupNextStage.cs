using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using TMPro;
using System.Numerics;

[UIPath("UI/Popup/PopupNextStage")]
public class PopupNextStage : UIBase
{
    [SerializeField]
    private Image BeforeImg;

    [SerializeField]
    private Image AfterFoodImg;

    [SerializeField]
    private TextMeshProUGUI BeforeFoodNameText;

    [SerializeField]
    private TextMeshProUGUI AfterFoodNameText;

    [SerializeField]
    private Slider UpgradeSlider;

    [SerializeField]
    private TextMeshProUGUI UpgradeCountText;

    [SerializeField]
    private TextMeshProUGUI NextStagePurchaseValueText;

    [SerializeField]
    private Button NextStageBtn;

    [SerializeField]
    private Button UpgradeBtn;
    private CompositeDisposable disposables = new CompositeDisposable();
    private BigInteger PurChaseMoney;

    protected override void Awake()
    {
        base.Awake();

        NextStageBtn.onClick.AddListener(OnClickNextStage);

        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }


    public void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => popup.Init());
    }

    public void OnClickNextStage()
    {
        Hide();
        GameRoot.Instance.InGameSystem.NextGameStage();
    }


    public void Init()
    {
        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx);

        var nextstagetd = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx + 1);

        if (td != null)
        {
            PurChaseMoney = td.next_stage_money;

            disposables.Clear();

            UpgradeSliderCheck();

            foreach (var upgrade in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
            {
                upgrade.IsBuyCheckProperty.SkipLatestValueOnSubscribe().Subscribe(x => {
                    if(x)
                    {
                        UpgradeSliderCheck();
                    }
                }).AddTo(disposables);
            }

            GameRoot.Instance.UserData.CurMode.Money.SkipLatestValueOnSubscribe().Subscribe(x => { UpgradeSliderCheck(); }).AddTo(disposables);

            BeforeImg.sprite = Config.Instance.GetIngameImg(td.nextstage_image);
            BeforeFoodNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.nextstage_name);

            NextStagePurchaseValueText.text = ProjectUtility.CalculateMoneyToString(PurChaseMoney);
        }


        if(nextstagetd != null)
        {
            AfterFoodNameText.text = Tables.Instance.GetTable<Localize>().GetString(nextstagetd.nextstage_name);
            AfterFoodImg.sprite = Config.Instance.GetIngameImg(nextstagetd.nextstage_image);
        }
    }

    public void UpgradeSliderCheck()
    {
        var upgradelist  = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList;

        var isbuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value);

        UpgradeSlider.value = (float)isbuylist.Count / (float)upgradelist.Count;

        UpgradeCountText.text = $"{isbuylist.Count}/{upgradelist.Count}";

        NextStageBtn.interactable = isbuylist.Count >= upgradelist.Count && GameRoot.Instance.UserData.CurMode.Money.Value >= PurChaseMoney;
    }
}
