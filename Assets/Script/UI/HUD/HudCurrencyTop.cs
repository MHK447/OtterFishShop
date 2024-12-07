using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;


public class HudCurrencyTop : MonoBehaviour
{
    [SerializeField]
    private Text CashText;

    [SerializeField]
    private Text MoneyText;


    private void Awake()
    {
        SyncData();
    }


    private void SyncData()
    {
        if (CashText != null)
        {
            CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();
            GameRoot.Instance.UserData.HUDCash.Subscribe(x =>
            {
                CashText.text = x.ToString();
            }).AddTo(this);

        }


        if (MoneyText != null)
        {
            MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

            GameRoot.Instance.UserData.HUDMoney.Subscribe(x =>
            {
                MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);
            }).AddTo(this);
        }

    }


}
