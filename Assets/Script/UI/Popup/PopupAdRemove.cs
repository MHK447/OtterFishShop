using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;


[UIPath("UI/Popup/PopupAdRemove")]
public class PopupAdRemove : UIBase
{
    [SerializeField]
    private TextMeshProUGUI PriceText;

    [SerializeField]
    private Button PurchaseBtn;


    protected override void Awake()
    {
        base.Awake();
        PurchaseBtn.onClick.AddListener(OnClickReward);
    }

    public void Init()
    {
        //text입력 
    }


    public void OnClickReward()
    {
        //결제
        GameRoot.Instance.ShopSystem.IsVipProperty.Value = true;
        Hide();
    }
}
