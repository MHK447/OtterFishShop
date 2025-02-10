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
    private List<Toggle> TabToggles = new List<Toggle>();

    private TabType defualtOption = TabType.ProductTab;

    protected override void Awake()
    {
        base.Awake();
        TopCurrencySync();


        int iter = 0;
        foreach (var toggle in TabToggles)
        {
            var tabIdx = iter;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(on =>
            {
                ChangeTab((TabType)tabIdx, on);
            });
            ++iter;
        }
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

        public void ChangeTab(TabType tab, bool on)
    {
        if (CurrentTab == tab) return;

        CurrentTab = tab;

        if (on)
        {
            SelectTab(tab);
        }


        foreach(var toggle in TabToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();

            toggleani.SetTrigger("Normal");
        }

        var ani = TabToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            if (on)
            {
                // if(IsInit)
                //     SoundPlayer.Instance.PlaySound("btn");

                if (!TabToggles[(int)tab].isOn)
                    TabToggles[(int)tab].isOn = true;
                ani.SetTrigger("Selected");
            }
            else
                ani.SetTrigger("Normal");
        }
    }



    public void SelectTab(TabType tab)
    {

        switch(tab)
        {
            case TabType.FacilityTab:
            OnClickFacility();
            break;
            case TabType.ProductTab:
            OnClickProduct();
            break;
        }

    }

    public override void OnShowBefore()
    {
        base.OnShowBefore();

        StartCoroutine(WaitOneFrame());
    }



    IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        var viewTab = defualtOption;
        if (!TabToggles[(int)defualtOption].isOn)
        {
            TabToggles[(int)defualtOption].isOn = true;
        }
        else
        {
            var ani = TabToggles[(int)defualtOption].gameObject.GetComponent<Animator>();
            if (ani != null)
            {
                ani.SetTrigger("Selected");
            }
            ChangeTab(defualtOption, true);
        }
    }



    IEnumerator OnShowWaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
      

            var ani = TabToggles[(int)defualtOption].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            ani.SetTrigger("Selected");
        }


    }


    public override void OnShowAfter()
    {
        base.OnShowAfter();

        StartCoroutine(OnShowWaitOneFrame());
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
