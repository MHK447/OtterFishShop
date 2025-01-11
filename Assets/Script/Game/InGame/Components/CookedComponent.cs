using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using Spine.Unity;
using System.Linq;

public class CookedComponent : FacilityComponent
{
    public enum State
    {
        Working,
        Break,
        Idle,
    }

    [SerializeField]
    private Transform ProgressTr;

    [SerializeField]
    private List<Transform> FoodTrList = new List<Transform>();

    [SerializeField]
    protected SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private List<CookedMaterialComponent> CookedMaterialList = new List<CookedMaterialComponent>();

    private Queue<FishComponent> FoodComponetQueue = new Queue<FishComponent>();

    private State CurState = State.Idle;

    private int MaterialMaxCount = 0;

    private float Cookeddeltime = 0f;

    private CooltimeProgress Progress;

    private List<OtterBase> ConsumerOtterList = new List<OtterBase>();

    private List<OtterBase> CasherOtterList = new List<OtterBase>();

    private float FishCarrydeltime = 0f;

    private float cookdeltime = 0f;

    private int FoodIdx = 0; 

    public override void Init()
    {
        base.Init();

        FoodComponetQueue.Clear();

        var td = Tables.Instance.GetTable<CookingInfo>().GetData(FacilityIdx);

        if(td != null)
        {
            ChangeState(State.Idle);

            MaterialMaxCount = td.material_max_count;

            FoodIdx = td.Food_idx;

            for (int i = 0; i < td.material_idxs.Count; ++i)
            {
                CookedMaterialList[i].Set(td.material_idxs[i], MaterialMaxCount);
            }

            GameRoot.Instance.UISystem.LoadFloatingUI<CooltimeProgress>((_progress) => {
                Progress = _progress;
                ProjectUtility.SetActiveCheck(Progress.gameObject, false);
                Progress.Init(ProgressTr);
                Progress.SetValue(0);
            });

        }
    }


    public void ChangeState(State state)
    {
        if (CurState == state) return;

        CurState = state;

        switch (CurState)
        {
            case State.Working:
                {
                    skeletonAnimation.state.SetAnimation(0, "work", true);
                }
                break;
            case State.Break:
                {
                    skeletonAnimation.state.SetAnimation(0, "break", true);
                }
                break;
            case State.Idle:
                {
                    skeletonAnimation.state.SetAnimation(0, "idle", true);
                }
                break;
        }

    }


    public void CoolTimeActive(float cooltimevalue)
    {
        if (Progress == null) return;

        if (cooltimevalue > 0f && !Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, true);
        }

        if (cooltimevalue <= 0f && Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }

        if (Progress.gameObject.activeSelf)
            Progress.SetValue(cooltimevalue);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Consumer")))
        {
            FishCarrydeltime = 0f;
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.GetFishComponentList.Count > 0)
            {
                if (!ConsumerOtterList.Contains(getvalue))
                {
                    ConsumerOtterList.Add(getvalue);
                }
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            cookdeltime = 0f;
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.GetFishComponentList.Count > 0)
            {
                if (!CasherOtterList.Contains(getvalue))
                {
                    CasherOtterList.Add(getvalue);
                }
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Consumer"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (ConsumerOtterList.Contains(getvalue))
                {
                    ConsumerOtterList.Remove(getvalue);
                }
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            cookdeltime = 0f;
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.GetFishComponentList.Count > 0)
            {
                if (CasherOtterList.Contains(getvalue))
                {
                    CasherOtterList.Remove(getvalue);
                }
            }
        }
    }


    public void CheckFoodMoveOtter()
    {
        if (FoodComponetQueue.Count == 0) return;

        for (int i = ConsumerOtterList.Count - 1; i >= 0; i--)
        {
            if (ConsumerOtterList[i].IsIdle && ConsumerOtterList[i].GetFishComponentList.Count > 0)
            {
                if (ConsumerOtterList[i].IsIdle && !ConsumerOtterList[i].IsMaxFishCheck()) 
                {
                    FishCarrydeltime += Time.deltaTime;

                    if (FishCarrydeltime >= 0.2f)
                    {
                        FishCarrydeltime = 0f;

                        var targetotter = ConsumerOtterList[i];

                        if (!targetotter.IsMaxFishCheck())
                        {
                            var food = FoodComponetQueue.Dequeue();

                            ConsumerOtterList[i].AddFish(food);
                        }

                    }
                }
            }
        }
    }



    public void CheckMaterialMoveOtter()
    {

        for (int i = CasherOtterList.Count - 1; i >= 0; i--)
        {
            if (CasherOtterList[i].IsIdle && CasherOtterList[i].GetFishComponentList.Count > 0)
            {
                if (CasherOtterList[i].IsIdle)
                {
                    var findfish = CasherOtterList[i].GetFishComponentList.Last();

                    if (findfish != null)
                    {
                        var finddata = CookedMaterialList.Find(x => x.GetFishIdx == findfish.GetFishIdx);

                        if(finddata != null)
                        {
                            cookdeltime += Time.deltaTime;

                            if(cookdeltime > 0.4f)
                            {
                                cookdeltime = 0f;

                                CasherOtterList[i].RemoveFish(findfish);

                                findfish.FishInBucketAction(finddata.GetCurFishTr(), (fish) => {
                                    fish.transform.SetParent(this.transform);
                                }, 0.2f);

                                finddata.AddMaterial(findfish);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ProduceFood()
    {
        if (IsMaxCountCheck()) return;

        if (!MaterialAllCountCheck()) return;

        // 음식을 만들기

        InGameStage.CreateFish(FoodTrList[FoodComponetQueue.Count], FoodIdx, FishComponent.State.Bucket, FoodCreateComplete);

        foreach(var material in CookedMaterialList)
        {
            material.RemoveMaterial();
        }

    }

    public void FoodCreateComplete(FishComponent fish)
    {
        FoodComponetQueue.Enqueue(fish);
    }
       

    public bool MaterialAllCountCheck()
    {
        foreach (var material in CookedMaterialList)
        {
            if (material.MaterialCount < 1)
            {
                return false;
            }
        }
        return true;
    }


    public override bool IsMaxCountCheck()
    {
        return FoodComponetQueue.Count >= MaterialMaxCount;
    }


    public override void Update()
    {
        base.Update();

        CheckFoodMoveOtter();
        CheckMaterialMoveOtter();
    }




}
