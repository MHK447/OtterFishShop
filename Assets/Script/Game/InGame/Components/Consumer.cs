using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;
using System.Linq;
using Spine.Unity;

public class Consumer : Chaser
{
    public enum CurState
    {
        Idle,
        Move,
        WaitProduct,
        Wait,
    }


    public class PatternOrderData
    {
        public int FacilityIdx = 0;
        public int Count = 0;


        public PatternOrderData(int facilityidx , int count)
        {
            FacilityIdx = facilityidx;
            Count = count;
        }
    }

    [SerializeField]
    private Transform OrderTr;

    public ConsumerMoveInfoData CurMoveInfoData;


    [SerializeField]
    private Transform ProductRoot;

    private ConsumerOrderUI ConsumerOrderUI;

    private int CurMissionCount = 0;

    private Queue<PatternOrderData> PatternOrderQueue = new Queue<PatternOrderData>();

    private IReactiveProperty<int> CurFacilityIdxProperty = new ReactiveProperty<int>();

    private IReactiveProperty<int> CurCountProperty = new ReactiveProperty<int>();

    private CompositeDisposable disposables = new CompositeDisposable();

    private CurState State = CurState.Idle;

    private RackComponent TargetRack;

    private Transform FacilityTarget;

    private int CurGoalValue = 0;

    private List<FishComponent> CurFishComponentList = new List<FishComponent>();

    private CounterComponent CounterComponent;

    private InGameStage Stage;

    public System.Action<bool> OnEnd = null;

    public bool IsArrivedCounter = false;

    public int CurCounterOrder = 0;

    private bool IsCounter = false;



    public override void Init(int idx)
    {
        FacilityTarget = null;
        TargetRack = null;
        IsCounter = false;
        IsArrivedCounter = false;

        CarryStart(false);
        Stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
        CounterComponent = Stage.FindFacilityTr(GameRoot.Instance.InGameSystem.CounterIdx).GetComponent<CounterComponent>();

        base.Init(idx);

        CurMissionCount = 0;

        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        CurMoveInfoData = GameRoot.Instance.FacilitySystem.CreatePattern(curstageidx);

        PatternOrderQueue.Clear();

        for(int i = 0; i < CurMoveInfoData.facilityidx.Count; ++i)
        {
            var newpatterndata = new PatternOrderData(CurMoveInfoData.facilityidx[i], CurMoveInfoData.count[i]);

            PatternOrderQueue.Enqueue(newpatterndata);
        }


        CurGoalValue = CurMoveInfoData.count[CurMissionCount];

        GameRoot.Instance.UISystem.LoadFloatingUI<ConsumerOrderUI>((orderui) => {
            ConsumerOrderUI = orderui;
            ProjectUtility.SetActiveCheck(ConsumerOrderUI.gameObject, true);
            orderui.Init(OrderTr);
            orderui.Set(this, CurMoveInfoData.facilityidx[CurMissionCount], 0 , CurMoveInfoData.count[CurMissionCount]);
        });

        disposables.Clear();

        CurCountProperty.Subscribe(x => {
            if(ConsumerOrderUI != null)
            {
                ConsumerOrderUI.SetCountText(x);
            }
        }).AddTo(disposables);

        MoveFacility();
    }



    public void MoveFacility()
    {
        if (PatternOrderQueue.Count > 0)
        {
            var newdata = PatternOrderQueue.Dequeue();

            CurFacilityIdxProperty.Value = newdata.FacilityIdx;
            CurGoalValue = newdata.Count;
            CurCountProperty.Value = 0;
            

            GoToFacility(newdata.FacilityIdx, ()=> {
                NextMoveAction(CurFacilityIdxProperty.Value);
            });
        }
        else
        {

            //lastmove

        }
    }


    public void NextMoveAction(int facilityidx)
    {
        if(facilityidx > 0 && facilityidx < 100) //기본 물품대 
        {
            ChangeState(CurState.WaitProduct, facilityidx);
        }
        else if(facilityidx > 99 && facilityidx < 1000) // 조리대 
        {

        }
        else if(facilityidx == 1000) //계산대
        {
            IsArrivedCounter = true;
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

    public void ChangeState(CurState state , int facilityidx = -1)
    {
        State = state;

        switch(State)
        {
            case CurState.Idle:
                break;
            case CurState.Move:
                break;
            case CurState.WaitProduct:
                {
                    var getfacility = Stage.FindFacilityTr(facilityidx);

                    if(getfacility != null)
                    {
                        TargetRack = getfacility.GetComponent<RackComponent>();
                    }

                }
                break;
        }
    }


    private void GoToFacility(int facilityidx , System.Action nextaction)
    {
        if(FacilityTarget != null)
        {
            Stage.FindFacilityTr(facilityidx).ConsumerOrder -= 1;
        }

        if(GameRoot.Instance.InGameSystem.CounterIdx == facilityidx)
        {
            FacilityTarget = CounterComponent.GetEmptyConsumerTr();

            IsCounter = true;

            CounterComponent.AddConsumer(this);
        }
        else
            FacilityTarget = Stage.GetFacilityConsumeTr(facilityidx);


        if (FacilityTarget != null)
        {
            PlayAnimation("move", true);
            SetDestination(FacilityTarget, nextaction);
        }
        else
        {
            //wait
        }
    }


    private void Update()
    {
        if(TargetRack != null && State == CurState.WaitProduct )
        {

            if(CurCountProperty.Value >= CurGoalValue)
            {
                MoveFacility();
            }
            else
            {
                if(TargetRack.GetFishComponentList.Count > 0)
                {
                    var target = TargetRack.GetFishComponentList.First();

                    TargetRack.GetFishComponentList.Remove(target);

                    AddFish(target);

                }
            }
        }

        if(IsCounter && CurCounterOrder > 0)
        {
            var finddata = CounterComponent.FindOrderConsumer(CurCounterOrder - 1);

            if(finddata == null)
            {
                CurCounterOrder -= 1;
                MovementCounterConsumer(CurCounterOrder , null);
            }
        }
    }



    public void CarryStart(bool iscarry)
    {
        IsCarry = iscarry;

    }

    private float FishPos_Y = 0.15f;

    public void AddFish(FishComponent fish)
    {
        CurCountProperty.Value += 1;
        CurFishComponentList.Add(fish);
        CarryStart(CurFishComponentList.Count > 0);

        PlayAnimation("carryidle", true);


        var floory = (FishPos_Y * (CurFishComponentList.Count - 1));

        fish.FishInBucketAction(ProductRoot, (fish) =>
        {
            fish.transform.SetParent(ProductRoot);
        }, 0.25f , floory);
    }



    public void OutCounterConsumer()
    {
        ProjectUtility.SetActiveCheck(ConsumerOrderUI.gameObject, false);
        SetDestination(Stage.GetMiddleEndTr , ()=> {
            SetDestination(Stage.GetEndTr , () => {
                DataClear();
                ProjectUtility.SetActiveCheck(this.gameObject, false);
                OnEnd?.Invoke(true);
            });
        });

    }

    public void DataClear()
    {
        CurFacilityIdxProperty.Value = 0;
        CurCountProperty.Value = 0;
        PatternOrderQueue.Clear();
        CurFishComponentList.Clear();
        IsCarry = false;
        CurCounterOrder = 0;
        FacilityTarget = null;
        TargetRack = null;
        IsCounter = false;
        IsArrivedCounter = false;

    }

    public void MovementCounterConsumer(int order , System.Action moveendaction)
    {
        if(CounterComponent != null)
        {
            var consumertr = CounterComponent.GetConsumerTr(order);

            SetDestination(consumertr, moveendaction);
        }
    }
}
