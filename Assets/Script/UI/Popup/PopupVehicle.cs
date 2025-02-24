using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using TMPro;

[UIPath("UI/Popup/PopupVehicle")]
public class PopupVehicle : UIBase
{
    [SerializeField]
    private TextMeshProUGUI MinuteText;

    [SerializeField]
    private TextMeshProUGUI CostText;


    [SerializeField]
    private Button AdBtn;


    [SerializeField]
    private Button CashBtn;

    [SerializeField]
    private TextMeshProUGUI BuffValueText;

    private int VehicleIdx = 0;

    protected override void Awake()
    {
        base.Awake();

        CashBtn.onClick.AddListener(OnClickCash);
        AdBtn.onClick.AddListener(OnClickAd);

    }

    public void Init()
    {
        MinuteText.text = Tables.Instance.GetTable<Localize>().GetFormat("minute_time", GameRoot.Instance.VehicleSystem.ad_ride_time);
        CostText.text = GameRoot.Instance.VehicleSystem.ride_cash_value.ToString();

        var td = Tables.Instance.GetTable<VehicleInfo>().GetData(1);

        if (td != null)
        {
            BuffValueText.text =  Tables.Instance.GetTable<Localize>().GetFormat("ad_vehicle_value",td.buff_value);
            
        }
    }


    public void OnClickCash()
    {

    }

    public void OnClickAd()
    {

    }

}
