using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

[UIPath("UI/Page/HUDTotal", true)]
public class HUDTotal : UIBase
{
    [SerializeField]
    private HudCurrencyTop CurrencyTop;


    [SerializeField]
    private Button UpgradeBtn;

    [SerializeField]
    private Button NextStageBtn;


    protected override void Awake()
    {
        base.Awake();
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
        NextStageBtn.onClick.AddListener(OnClickNextStage);
    }

    public void OnClickNextStage()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupNextStage>(popup => popup.Init());
    }


    public void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => popup.Init());
    }

}
