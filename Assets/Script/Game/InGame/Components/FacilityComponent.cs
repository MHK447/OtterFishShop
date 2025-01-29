using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;
using System.Linq;

public class FacilityComponent : MonoBehaviour
{
    [SerializeField]
    protected BoxCollider2D Col;

    
    [SerializeField]
    private List<GameObject> FacilityOpenList = new List<GameObject>();

    [SerializeField]
    private GameObject FacilityContentsObj;

    [SerializeField]
    private SpriteRenderer FacilitySprite;

    [SerializeField]
    private Transform MoneyRootTr;

    [SerializeField]
    protected List<Transform> ConsumerWaitTr = new List<Transform>();

    [SerializeField]
    private Transform NewRoot;

    protected OtterBase Player;

    public int ConsumerOrder = 0;

    public int FacilityIdx = 0;

    protected int CapacityMaxCount = 0;

    private NewFacilityUI NewFacilityUI;

    protected FacilityData FacilityData;

    public FacilityData GetFacilityData { get { return FacilityData; } }

    private bool OnEnter = false;

    private int MoneySpeedCount = 0;

    private float moneydeltime =0f;

    private int GoalCount = 0;

    private int FacilityOpenOrder = 0;

    private CompositeDisposable disposables = new CompositeDisposable();

    protected InGameStage InGameStage;

    protected int BaseCapacity = 0;

    private float FacilityOpenSpeed = 0.2f;

    public virtual void Init()
    {
        moneydeltime = 0.1f;

        ConsumerOrder = 0;

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);

        var ingametycoon = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();

        InGameStage = ingametycoon.curInGameStage;

        Player = ingametycoon.GetPlayer;

        ProjectUtility.SetActiveCheck(FacilityContentsObj, false);

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityIdx);

        BaseCapacity = facilitytd.start_capacity;

        var stagefacilitytd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.Where(x => x.facilityidx == FacilityIdx
        && x.stageidx == stageidx).FirstOrDefault();

        if (stagefacilitytd == null) return;

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.ShelfCapacityUp,FacilityIdx);

        CapacityMaxCount = BaseCapacity + (int)buffvalue;

     

        if (!FacilityData.IsOpen)
        {
            var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;


            FacilityOpenOrder = Tables.Instance.GetTable<FacilityOpenOrder>().DataList.ToList().Find(x => x.stageidx == curstageidx
            && FacilityIdx == x.facilityidx).openorder;

            var openorder = GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty;


            openorder.SkipLatestValueOnSubscribe().Subscribe(x => {
                if(NewFacilityUI != null)
                {
                    if(!FacilityData.IsOpen && FacilityOpenOrder == openorder.Value)
                    {
                        GameRoot.Instance.WaitTimeAndCallback(1f, () => {
                            GameRoot.Instance.InGameSystem.CurInGame.IngameCamera.FoucsPosition(NewFacilityUI.transform);
                        });
                        GameRoot.Instance.WaitTimeAndCallback(3f, () => {
                            GameRoot.Instance.InGameSystem.CurInGame.IngameCamera.FocusOff();
                        });
                    }

                    ProjectUtility.SetActiveCheck(FacilityContentsObj, !FacilityData.IsOpen
                                    && FacilityOpenOrder == openorder.Value);

                    ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, x == FacilityOpenOrder &&
                        !FacilityData.IsOpen);
                }
            }).AddTo(disposables);

            ProjectUtility.SetActiveCheck(FacilityContentsObj, !FacilityData.IsOpen
                            && FacilityOpenOrder == openorder.Value);

            if (facilitytd != null)
            {
                FacilitySprite.sprite = Config.Instance.GetIngameImg(facilitytd.image);

                GoalCount = stagefacilitytd.open_cost;


                if (NewFacilityUI == null)
                {
                    GameRoot.Instance.UISystem.LoadFloatingUI<NewFacilityUI>((_newfacility) =>
                    {
                        NewFacilityUI = _newfacility;

                        ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, !FacilityData.IsOpen
                            && FacilityOpenOrder == openorder.Value);

                        _newfacility.Init(NewRoot);
                        _newfacility.SliderValue(FacilityData.MoneyCount, stagefacilitytd.open_cost);
                    });
                }
            }
        }

        Col.isTrigger = !FacilityData.IsOpen;


        foreach (var facility in FacilityOpenList)
        {
            ProjectUtility.SetActiveCheck(facility, FacilityData.IsOpen);
        }

    }

    public virtual Transform GetConsumerTr()
    {
        if (ConsumerWaitTr.Count == 0) return null; 

        var randvalue = Random.Range(0, ConsumerWaitTr.Count);
        return ConsumerWaitTr[randvalue];

    }

    


    public virtual bool IsMaxCountCheck()
    {
        if (FacilityData == null) return false;


        return FacilityData.CapacityCountProperty.Value >= CapacityMaxCount;
    }
    

    public Transform GetConsumerTr(int order)
    {
        return ConsumerWaitTr[order];
    }

    public bool IsOpenFacility()
    {
        if (FacilityData == null) return false;


        return FacilityData.IsOpen;
    }



    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player")) && !FacilityData.IsOpen)
        {
            if(NewFacilityUI != null && NewFacilityUI.gameObject.activeSelf)
            OnEnter = true;
            MoneySpeedCount = 0;
            FacilityOpenSpeed = 0.2f;
        }
    }
        
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player")) && !FacilityData.IsOpen)
        {
            OnEnter = false;
        }
    }

    public void OpenFacility()
    {
        ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, false);
        FacilityData.IsOpen = true;
        GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty.Value += 1;
        Init();

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var stageinfotd = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        if (stageinfotd != null)
        {
            if (stageinfotd.consumerfirst_idx == FacilityIdx)
            {
                var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.AddCustomer);

                for (int i = 0; i < upgradevalue; ++i)
                {
                    InGameStage.CreateConsumer(1, InGameStage.GetStartWayPoint);
                }
            }
        }
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
    }



    public virtual void Update()
    {
        if (FacilityData != null && !FacilityData.IsOpen && OnEnter)
        {
            if (GameRoot.Instance.UserData.CurMode.Money.Value > 0)
            {
                moneydeltime += Time.deltaTime;

                if (moneydeltime >= FacilityOpenSpeed)
                {
                    MoneySpeedCount += 1;

                    FacilityOpenSpeed -= 0.02f;

                    GameRoot.Instance.EffectSystem.MultiPlay<MoneyEffect>(Player.transform.position, effect =>
                    {
                        effect.SetAutoRemove(true, 1f);
                        effect.Init(MoneyRootTr, ()=> {
                            ProjectUtility.SetActiveCheck(effect.gameObject, false);

                        });
                    });

                    moneydeltime = 0f;

                    GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -1);

                    FacilityData.MoneyCount += 1;

                    NewFacilityUI.SliderValue(FacilityData.MoneyCount, GoalCount);

                    if (FacilityData.MoneyCount >= GoalCount)
                    {
                        OpenFacility();
                    }
                }
            }
        }
    }
}
