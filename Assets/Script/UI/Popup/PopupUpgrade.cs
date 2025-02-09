using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;

[UIPath("UI/Popup/PopupUpgrade")]
public class PopupUpgrade : UIBase
{
    public enum TabType
    {
        ProductTab,
        FacilityTab,
    }


    [SerializeField]
    private UpgradeProductComponentGroup ProductComponentGroup;

    [SerializeField]
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private GameObject CachedPrefab;

    [SerializeField]
    private Transform CachedRoot;

    [SerializeField]
    private GameObject ProductRoot;

    [SerializeField]
    private GameObject FacilityRoot;

    private TabType CurrentTab = TabType.ProductTab;

    [SerializeField]
    private Button FacilityBtn;

    [SerializeField]
    private Button ProductBtn;

    protected override void Awake()
    {
        base.Awake();
        FacilityBtn.onClick.AddListener(OnClickFacility);
        ProductBtn.onClick.AddListener(OnClickProduct);
        TopCurrencySync();
    }

    public void OnClickFacility()
    {
        CurrentTab = TabType.FacilityTab;

        ProjectUtility.SetActiveCheck(ProductRoot, CurrentTab == TabType.ProductTab);
        ProjectUtility.SetActiveCheck(FacilityRoot , CurrentTab == TabType.FacilityTab);


    }

    public void OnClickProduct()
    {
        CurrentTab = TabType.ProductTab;

        ProjectUtility.SetActiveCheck(ProductRoot, CurrentTab == TabType.ProductTab);
        ProjectUtility.SetActiveCheck(FacilityRoot , CurrentTab == TabType.FacilityTab);
    }

    public void Init()
    {
        CurrentTab = TabType.ProductTab;

        ProductComponentGroup.Init();   

        OnClickProduct();

        ProjectUtility.SetActiveCheck(ProductRoot.gameObject , CurrentTab == TabType.ProductTab);
        ProjectUtility.SetActiveCheck(FacilityRoot.gameObject , CurrentTab == TabType.FacilityTab);

        foreach(var cachedobj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(cachedobj, false);
        }


        foreach(var upgradedata in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
        {
            if (upgradedata.IsBuyCheckProperty.Value) continue;

            var getobj = GetCachedObject().GetComponent<UpgradeComponent>();

            if(getobj != null)
            {
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                getobj.Set(upgradedata.UpgradeIdx);
            }
        }
    }


    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(CachedPrefab);
            inst.transform.SetParent(CachedRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
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


    public override void Hide()
    {
        base.Hide();


        var getui = GameRoot.Instance.UISystem.GetUI<PopupNextStage>();

        if(getui !=  null)
        {
            getui.UpgradeSliderCheck();
        }
    }
}
