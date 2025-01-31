using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.AI;
using Spine.Unity;
using UniRx;

public class CarryCasher : OtterBase
{
    private float waitdeltime = 0f;

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    private CompositeDisposable disposables = new CompositeDisposable();

    private float sleepdeltime = 0f;


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

        SetCapacity();

        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.TransportStaffSpeedUp);

        var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);


        CasherMoveSpeed = GameRoot.Instance.InGameSystem.casher_move_speed + getcalcvalue;

        var donebuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);

        disposables.Clear();

        foreach (var donebuy in donebuylist)
        {
            donebuy.IsBuyCheckProperty.Subscribe(x => {
                if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.TransportStaffSpeedUp)
                {
                    var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.TransportStaffSpeedUp);

                    var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);

                    CasherMoveSpeed = GameRoot.Instance.InGameSystem.casher_move_speed + getcalcvalue;
                }
            }).AddTo(disposables);
        }
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
       .Where(x => x.IsOpen && (x.FacilityIdx > 100)).ToList();


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

            if (HandleFishCookedDisplay(facility.FacilityIdx))
            {
                return true;
            }

            if(HandleFishDisplay(facility.FacilityIdx, mainFacility))
            {
                return true;
            }


        }

        return false;
    }
        

    private bool HandleFishDisplay(int facilityidx, FacilityComponent mainFacility)
    {
        var rackfacility = CurStage.FindFacility(facilityidx - 100);
        if (rackfacility == null) return false;

        if (rackfacility.IsMaxCountCheck()) return false;
        
        var fishRoom = mainFacility.GetComponent<FishRoomComponent>();
        if (fishRoom == null || mainFacility.GetFacilityData.CapacityCountProperty.Value <= 0) return false;

        EnqueueFishDisplayActions(fishRoom , rackfacility);

        return true;
    }


    private bool HandleFishCookedDisplay(int facilityidx)
    {
        var cookedfacility = CurStage.FindFacility(facilityidx);
        if (cookedfacility == null) return false;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd == null) return false;

        var cookcomponent = cookedfacility.GetComponent<CookedComponent>();

        if (cookcomponent == null) return false;

        var cookedtd = Tables.Instance.GetTable<CookingInfo>().GetData(cookcomponent.FacilityIdx);

        if (cookedtd == null) return false;

        foreach(var materialidx in cookedtd.material_idxs)
        {
            if(!cookcomponent.IsMaterialMaxCheck(materialidx))
            {
                var findfishroom = CurStage.FindFacility(materialidx + 100);

                if(findfishroom != null)
                {
                    var fishRoom = findfishroom.GetComponent<FishRoomComponent>();

                    if (fishRoom != null)
                    {
                        EnqueueFishDisplayActions(fishRoom, cookcomponent);
                        return true;
                    }
                }
            }
        }

        return false;
    }



    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom, CookedComponent targetdisplay)
    {
        System.Action moveToBucket = () =>
        {
            SetDestination(fishRoom.GetBucketComponent.transform, () => {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToBucket);

        System.Action moveToDisplay = () =>
        {
            SetDestination(targetdisplay.GetCarryCasherWaitTr, () => {
                PlayAnimation(OtterState.Idle, "idle", true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction, targetdisplay));
        };
        WorkActionQueue.Enqueue(moveToDisplay);

        System.Action WaitToWork = () =>
        {
            if (FishComponentList.Count > 0)
            {
                GameRoot.Instance.StartCoroutine(CheckWaitTrashCan(() => {
                    PlayAnimation(OtterState.Wait, "idle", true);
                }));
            }
            else
                PlayAnimation(OtterState.Wait, "idle", true);
        };
        WorkActionQueue.Enqueue(WaitToWork);
    }


    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom , FacilityComponent targetdisplay)
    {
        System.Action moveToBucket = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            SetDestination(fishRoom.GetBucketComponent.transform, ()=> {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToBucket);

        var rackcomponent = targetdisplay.GetComponent<RackComponent>();

        System.Action moveToDisplay = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            SetDestination(rackcomponent.GetCarryCasherWaitTr(this.transform), () => {
                PlayAnimation(OtterState.Idle, "idle" , true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction, rackcomponent));
        };
        WorkActionQueue.Enqueue(moveToDisplay);

        System.Action WaitToWork = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            if (FishComponentList.Count > 0)
            {
                GameRoot.Instance.StartCoroutine(CheckWaitTrashCan(() => {
                    PlayAnimation(OtterState.Wait, "idle", true);
                }));
            }
            else
                PlayAnimation(OtterState.Wait, "idle", true);
        };
        WorkActionQueue.Enqueue(WaitToWork);
    }

    public void NextWorkAction()
    {
        if(WorkActionQueue.Count > 0)
        {
            var nextaction = WorkActionQueue.Dequeue();

            nextaction?.Invoke();
        }
    }

    public void StartWorkCheck()
    {
        if (CurState == OtterState.Sleep || CurState == OtterState.SleepMove) return;

        if (CurState == OtterState.Wait || (CurState == OtterState.Idle && FishComponentList.Count == 0 && WorkActionQueue.Count == 0))
        {
            waitdeltime += Time.deltaTime;

            if (waitdeltime >= 1f)
            {
                waitdeltime = 0f;

                StartWork();
            }
        }
    }

    public override void Update()
    {
        base.Update();

        StartWorkCheck();


        sleepdeltime += Time.deltaTime;


        if (sleepdeltime >= GameRoot.Instance.InGameSystem.carry_sleep_time)
        {
            if (CurState != OtterState.Sleep && CurState != OtterState.SleepMove && FishComponentList.Count == 0)
            {
                WorkActionQueue.Clear();
                sleepdeltime = 0f;
                ChangeState(OtterState.SleepMove);
                SetDestination(CurStage.CarrySleepTr, () => {
                    PlayAnimation(OtterState.Sleep, "napstart", false);
                });
            }
        }
    }

    public void GoToTrashCan(System.Action endaction)
    {
        SetDestination(CurStage.GetTrashCanComponent.GetConsumerTr, () => {
            endaction?.Invoke();
        });
    }

    private float CheckDuration = 5f;

    private IEnumerator CheckWaitProductMax(System.Action nextaction)
    {
        float elapsedTime = 0f;

        while (elapsedTime < CheckDuration)
        {
            if (FishComponentList.Count >= StartCarryCount)
            {
                nextaction?.Invoke();
                yield break; // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        nextaction?.Invoke();
    }

    private IEnumerator CheckWaitProductNone(System.Action nextaction , RackComponent rackComponent)
    {
        if (FishComponentList.Count == 0 || rackComponent.IsMaxCountCheck())
        {
            nextaction?.Invoke();
            yield break;
        }
        yield return new WaitUntil(() => FishComponentList.Count == 0 || rackComponent.IsMaxCountCheck());
        nextaction?.Invoke();
    }



    private IEnumerator CheckWaitProductNone(System.Action nextaction, CookedComponent cookedcomponent)
    {
        if (FishComponentList.Count == 0)
        {
            nextaction?.Invoke();
            yield break;
        }

        var fishidx = FishComponentList[0].GetFishIdx;

        if (cookedcomponent.IsMaterialMaxCheck(fishidx))
        {
            nextaction?.Invoke();
            yield break;
        }

        yield return new WaitUntil(() => cookedcomponent.IsMaterialMaxCheck(fishidx) || FishComponentList.Count <= 0);
        nextaction?.Invoke();
    }



    private IEnumerator CheckWaitTrashCan(System.Action nextaction)
    {
        if (FishComponentList.Count == 0)
        {
            nextaction?.Invoke();
            yield break;
        }
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
                    skeletonAnimation.state.SetAnimation(0, "napidle", true);
                }
                break;
            case "napend":
                {
                    IsSleepStart = false;   
                    PlayAnimation(OtterState.Idle, "idle", true);
                }
                break;
        }
        AnimAction?.Invoke();

        AnimAction = null;
    }

    private bool IsSleepStart = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CurState == OtterState.Sleep && !IsSleepStart)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                sleepdeltime = 0f;

                IsSleepStart = true;

                skeletonAnimation.state.SetAnimation(0, "napend", false);
            }
        }
        
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
}
