using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;
using UniRx;


public class FishCasher : OtterBase
{
    private Transform TargetComponent;

    private float waitdeltime = 0f;

    private int MaxProductCount = 5;

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    Coroutine _currentMoveProcess;

    WaitForSeconds _waitTick;

    private int FacilityIdx = 0;

    public int GetFacilityIdx { get { return FacilityIdx; } }

    private FishRoomComponent FishRoomComponent;

    private CompositeDisposable disposables = new CompositeDisposable();

    public override void Init()
    {
        base.Init();


        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

        CurState = OtterState.Idle;

        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FishComponentList.Clear();

        WorkActionQueue.Clear();
    }


    public void Set(int facilityidx)
    {
        FacilityIdx = facilityidx;

        CurState = OtterState.Sleep;


        var findfacility = CurStage.FindFacility(facilityidx);

        if(findfacility != null)
        {
            FishRoomComponent = findfacility.GetComponent<FishRoomComponent>();

            disposables.Clear();

            FishRoomComponent.GetFacilityData.CapacityCountProperty.SkipLatestValueOnSubscribe().Subscribe(x => {
                if(FishRoomComponent.IsMaxCountCheck())
                {
                    PlayAnimation(OtterState.Sleep, "napstart", false);
                }
                else
                {
                    PlayAnimation(OtterState.Idle, "fishingidle", false);
                }

            }).AddTo(disposables);
        }

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });

        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;
    }

    public void StartWork()
    {
        SetDestination(FishRoomComponent.GetCushionComponent.transform, () => {
            if (FishRoomComponent.IsMaxCountCheck())
            {
                PlayAnimation(OtterState.Sleep, "napstart", false);
            }
            else
            {
                PlayAnimation(OtterState.Idle, "fishingidle", false);
            }
        });
    }
        
    private void OnDestroy()
    {
        disposables.Clear();

        // 콜백 해제
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.End -= HandleEvent;
        }
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

    public override void AddFish(FishComponent fish)
    {
    }


    private void HandleEvent(Spine.TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case "fishingstart":
                {
                    PlayAnimation(OtterState.Fishing, "fishingidle", true);
                }
                break;
            case "napstart":
                {
                    PlayAnimation(OtterState.Fishing, "napidle", true);
                }
                break;
            case "napend":
                {
                    PlayAnimation(OtterState.Fishing, "fishingstart", true);
                }
                break;
        }
        AnimAction?.Invoke();

        AnimAction = null;
    }



}
