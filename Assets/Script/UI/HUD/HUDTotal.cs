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
    private Button UpgradeBtn;

    [SerializeField]
    private Button NextStageBtn;

    [SerializeField]
    private Text FpsText;

    public Transform GetUpgradeBtnTr {get {return UpgradeBtn.transform; }}

    private float deltaTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
        NextStageBtn.onClick.AddListener(OnClickNextStage);
        TopCurrencySync();

        ProjectUtility.SetActiveCheck(UpgradeBtn.gameObject , false);
    }

    public void OnClickNextStage()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupNextStage>(popup => popup.Init());
    }


    public void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => popup.Init());
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FpsText.text = $"FPS: {Mathf.CeilToInt(fps)}";
    }


    public override void TopCurrencySync()
    {
        base.TopCurrencySync();

        if (CurrencyTop.CashText != null)
        {
            CurrencyTop.CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();
            GameRoot.Instance.UserData.HUDCash.Subscribe(x=>
            {
                CurrencyTop.CashText.text = x.ToString();
            }).AddTo(this);


                CurrencyTop.CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();

        }


        if (CurrencyTop.MoneyText != null)
        {
            CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

            GameRoot.Instance.UserData.HUDMoney.Subscribe(x =>
            {
                
                CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);
            }).AddTo(this);
        }

        CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

    }
}
