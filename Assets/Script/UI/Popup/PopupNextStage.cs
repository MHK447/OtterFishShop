using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using TMPro;

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
        //nextstage  
    }


    public void Init()
    {
        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx);

        var nextstagetd = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx + 1);

        if (td != null)
        {
            disposables.Clear();

            foreach (var upgrade in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
            {
                upgrade.IsBuyCheckProperty.SkipLatestValueOnSubscribe().Subscribe(x => {
                    if(x)
                    {
                        UpgradeSliderCheck();
                    }
                }).AddTo(disposables);
            }


        }
    }


    public void UpgradeSliderCheck()
    {
        var upgradelist  = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList;

        var isbuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value);

        UpgradeSlider.value = (float)isbuylist.Count / (float)upgradelist.Count;

        UpgradeCountText.text = $"{isbuylist.Count}/{upgradelist.Count}";

    }

}
