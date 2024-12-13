using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;

public class Consumer : Chaser
{
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

    private ConsumerOrderUI ConsumerOrderUI;

    private int CurMissionCount = 0;

    private Queue<PatternOrderData> PatternOrderQueue = new Queue<PatternOrderData>();

    private IReactiveProperty<int> CurFacilityIdxProperty = new ReactiveProperty<int>();

    private IReactiveProperty<int> CurCountProperty = new ReactiveProperty<int>();

    private CompositeDisposable disposables = new CompositeDisposable();

    public override void Init(int idx)
    {
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
            CurCountProperty.Value = newdata.Count;

            GoToFacility(newdata.FacilityIdx);
        }
        else
        {

            //lastmove

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


    private void GoToFacility(int facilityidx)
    {
        var facilitytr = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetFacilityTr(facilityidx);


        if (facilitytr != null)
            SetDestination(facilitytr , null);
        else
        {
            //wait
        }
    }

}
