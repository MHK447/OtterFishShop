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

    private float moneydeltime =0f;

    private int GoalCount = 0;

    private int FacilityOpenOrder = 0;

    private CompositeDisposable disposables = new CompositeDisposable();

    protected InGameStage InGameStage;

    public virtual void Init()
    {
        moneydeltime = 0.1f;

        ConsumerOrder = 0;

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);

        var ingametycoon = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();

        InGameStage = ingametycoon.curInGameStage;

        Player = ingametycoon.GetPlayer;

        ProjectUtility.SetActiveCheck(FacilityContentsObj, false);

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityIdx);

        CapacityMaxCount = facilitytd.initial_count;

        if (!FacilityData.IsOpen)
        {
            var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;


            FacilityOpenOrder = Tables.Instance.GetTable<FacilityOpenOrder>().DataList.ToList().Find(x => x.stageidx == curstageidx
            && FacilityIdx == x.facilityidx).openorder;

            var openorder = GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty;

            disposables.Clear();

            openorder.Subscribe(x => {
                if(NewFacilityUI != null)
                {

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

                GoalCount = facilitytd.initial_count;


                if (NewFacilityUI == null)
                {
                    GameRoot.Instance.UISystem.LoadFloatingUI<NewFacilityUI>((_newfacility) =>
                    {
                        NewFacilityUI = _newfacility;

                        ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, !FacilityData.IsOpen
                            && FacilityOpenOrder == openorder.Value);

                        _newfacility.Init(NewRoot);
                        _newfacility.SliderValue(FacilityData.MoneyCount, facilitytd.initial_count);
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
        ConsumerOrder += 1;
        return ConsumerWaitTr[ConsumerOrder - 1];

    }


    public bool IsMaxCountCheck()
    {
        if (FacilityData == null) return false;


        return FacilityData.CapacityCountProperty.Value >= CapacityMaxCount;
    }
    

    public Transform GetConsumerTr(int order)
    {
        return ConsumerWaitTr[order];
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !FacilityData.IsOpen)
        {
            if(NewFacilityUI != null && NewFacilityUI.gameObject.activeSelf)
            OnEnter = true;
        }
    }
        
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !FacilityData.IsOpen)
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

                if (moneydeltime >= 0.2f)
                {
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
