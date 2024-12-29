using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class CarryChaser : Chaser
{

    public enum State
    {
        Idle,
        Wait,
        Sleep,
        Work,
    }


    public State CurState = State.Idle;

    private InGameStage CurStage;

    private Transform TargetComponent;

    private float waitdeltime = 0f;

    private int MaxProductCount = 5;

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    public override void Init(int idx)
    {
        base.Init(idx);

        CurState = State.Idle;

        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FishComponentList.Clear();

        WorkActionQueue.Clear();

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });
    }




    public void StartWork()
    {
        if(TargetWorkFacility())
        {
            ChangeState(State.Work);
            NextWorkAction();
        }
        else
        {
            waitdeltime = 0f;
            ChangeState(State.Wait);
        }
    }





    public void ChangeState(State state)
    {
        if (CurState == state) return;


        CurState = state;
    }




    public bool TargetWorkFacility()
    {
        var facilitydatas = GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList
       .Where(x => x.IsOpen && x.FacilityIdx != 1000);


        foreach (var facility in facilitydatas)
        {
            var facilityInfo = Tables.Instance.GetTable<FacilityInfo>().GetData(facility.FacilityIdx);
            if (facilityInfo == null) continue;

            var mainFacility = CurStage.FindFacility(facilityInfo.facilityidx);
            if (mainFacility == null) continue;

            switch ((FacilitySystem.FacilityType)facilityInfo.facility_type)
            {
                case FacilitySystem.FacilityType.FishDisplay:
                    return HandleFishDisplay(facility.FacilityIdx, mainFacility);

                case FacilitySystem.FacilityType.Fishing:
                    return HandleFishing(mainFacility);

                case FacilitySystem.FacilityType.Counter:
                case FacilitySystem.FacilityType.Cooked:
                    break;
            }
        }

        return false;
    }


    private bool HandleFishDisplay(int facilityidx, FacilityComponent mainFacility)
    {
        var fishFacility = CurStage.FindFacility(facilityidx + 100);
        if (fishFacility == null || mainFacility.IsMaxCountCheck()) return false;

        var fishRoom = fishFacility.GetComponent<FishRoomComponent>();
        if (fishRoom == null || fishFacility.GetFacilityData.CapacityCountProperty.Value <= 0) return false;

        EnqueueFishDisplayActions(fishRoom);

        return true;
    }


    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom)
    {
        System.Action moveToBucket = () =>
        {
            SetDestination(fishRoom.GetBucketComponent.transform, null);
            GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
        };
        WorkActionQueue.Enqueue(moveToBucket);

        System.Action moveToDisplay = () =>
        {
            SetDestination(fishRoom.GetBucketComponent.transform, null);
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction));
        };
        WorkActionQueue.Enqueue(moveToDisplay);
    }

    private bool HandleFishing(FacilityComponent mainFacility)
    {
        if (mainFacility.IsMaxCountCheck()) return false;

        var fishRoom = mainFacility.GetComponent<FishRoomComponent>();
        
      
        System.Action moveTofishroom = () =>
        {
            SetDestination(fishRoom.GetCushionComponent.transform, null);
            GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
        };

        WorkActionQueue.Enqueue(moveTofishroom);


        return true;
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
        if(CurState == State.Wait)
        {
            waitdeltime += Time.deltaTime;

            if (waitdeltime >= 1f)
            {
                waitdeltime = 0f;

                StartWork();
            }
        }
    }

    private float CheckDuration = 2f;

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


}
