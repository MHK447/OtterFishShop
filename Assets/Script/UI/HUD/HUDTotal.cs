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


    protected override void Awake()
    {
        base.Awake();
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }


    public void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => popup.Init());
    }

}
