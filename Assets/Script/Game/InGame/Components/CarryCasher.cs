using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.AI;
using Spine.Unity;


public class CarryCasher : OtterBase
{
    private float waitdeltime = 0f;

    private int MaxProductCount = 5;

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    public override void Init()
    {
        base.Init();

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

        CurState = OtterState.Idle;
        
        FishComponentList.Clear();

        WorkActionQueue.Clear();

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });

        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;
    }




    public void StartWork()
    {
        if(TargetWorkFacility())
        {
            ChangeState(OtterState.Work);
            NextWorkAction();
        }
        else
        {
            waitdeltime = 0f;
            ChangeState(OtterState.Wait);

        }
    }

    public bool TargetWorkFacility()
    {
        var facilitydatas = GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList
       .Where(x => x.IsOpen && (x.FacilityIdx > 100 && x.FacilityIdx < 1000)).ToList();


        foreach (var facility in facilitydatas)
        {
            var facilityInfo = Tables.Instance.GetTable<FacilityInfo>().GetData(facility.FacilityIdx);
            if (facilityInfo == null) continue;

            if(CurStage == null)
            {
                CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
            }

            var mainFacility = CurStage.FindFacility(facilityInfo.facilityidx);
            if (mainFacility == null) continue;

            if(HandleFishDisplay(facility.FacilityIdx, mainFacility))
            {
                return true;
            }
        }

        return false;
    }


    private bool HandleFishDisplay(int facilityidx, FacilityComponent mainFacility)
    {
        var fishFacility = CurStage.FindFacility(facilityidx - 100);
        if (fishFacility == null || mainFacility.IsMaxCountCheck()) return false;

        var fishRoom = mainFacility.GetComponent<FishRoomComponent>();
        if (fishRoom == null || mainFacility.GetFacilityData.CapacityCountProperty.Value <= 0) return false;

        EnqueueFishDisplayActions(fishRoom , fishFacility);

        return true;
    }


    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom , FacilityComponent targetdisplay)
    {
        System.Action moveToBucket = () =>
        {
            SetDestination(fishRoom.GetBucketComponent.transform, ()=> {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToBucket);

        var rackcomponent = targetdisplay.GetComponent<RackComponent>();

        System.Action moveToDisplay = () =>
        {
            SetDestination(rackcomponent.GetCarryCasherWaitTr(this.transform), () => {
                PlayAnimation(OtterState.Idle, "idle" , true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction));
        };
        WorkActionQueue.Enqueue(moveToDisplay);
    }

    public void NextWorkAction()
    {
        if(WorkActionQueue.Count > 0)
        {
            var nextaction = WorkActionQueue.Dequeue();

            nextaction?.Invoke();
        }
    }

    private void Update()
    {
        if(CurState == OtterState.Wait)
        {
            waitdeltime += Time.deltaTime;

            if (waitdeltime >= 1f)
            {
                waitdeltime = 0f;

                StartWork();
            }
        }
    }

    private float CheckDuration = 5f;

    private IEnumerator CheckWaitProductMax(System.Action nextaction)
    {
        float elapsedTime = 0f;

        while (elapsedTime < CheckDuration)
        {
            if (FishComponentList.Count >= MaxProductCount)
            {
                nextaction?.Invoke();
                yield break; // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        nextaction?.Invoke();
    }

    private IEnumerator CheckWaitProductNone(System.Action nextaction)
    {
        yield return new WaitUntil(() => FishComponentList.Count == 0);

        nextaction?.Invoke();
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



    private void OnDestroy()
    {
        // 콜백 해제
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.End -= HandleEvent;
        }
    }
}
