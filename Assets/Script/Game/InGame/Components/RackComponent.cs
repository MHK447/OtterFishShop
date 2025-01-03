using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UniRx;

public class RackComponent : FacilityComponent
{
    [SerializeField]
    private int FishIdx = 0;

    [SerializeField]
    private List<Transform> FishTrList = new List<Transform>();

    [SerializeField]
    private Transform AmountUITr;

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    public List<FishComponent> GetFishComponentList { get { return FishComponentList; } }

    private List<OtterBase> TargetOtterList = new List<OtterBase>();

    private float FishCarrydeltime = 0f;
        
    private float FishCarryTime = 0.2f;

    private UI_AmountBubble AmountUI = null;

    private CompositeDisposable disposables = new CompositeDisposable();

    public override void Init()
    {
        base.Init();

        TargetOtterList.Clear();

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);


        GameRoot.Instance.UISystem.LoadFloatingUI<UI_AmountBubble>((_progress) => {
            AmountUI = _progress;
            ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.CapacityCountProperty.Value > 0);
            AmountUI.Init(AmountUITr);
            AmountUI.Set(FacilityData.FacilityIdx);
            AmountUI.SetValue(FacilityData.CapacityCountProperty.Value);
        });


        FacilityData.CapacityCountProperty.Subscribe(x => {
            if (AmountUI != null)
            {   
                AmountUI.SetValue(x);
            }
        }).AddTo(disposables);


        GameRoot.Instance.StartCoroutine(WaitOneFrame());
    }


    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < FacilityData.CapacityCountProperty.Value; ++i)
        {
            InGameStage.CreateFish(this.transform, 1, FishComponent.State.Rack, (fish) => {
                fish.FishInBucketAction(FishTrList[i], (fish) => {
                    fish.transform.SetParent(this.transform);
                    FishComponentList.Add(fish);
                }, 0f);
            });
        }


    }

    public void RemoveFish()
    {
        var target = FishComponentList.First();

        FishComponentList.Remove(target);

        FacilityData.CapacityCountProperty.Value -= 1;
    }

    public Transform GetCarryCasherWaitTr(Transform carrycashertr)
    {
        Transform closestTransform = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform waitTr in ConsumerWaitTr)
        {
            float distance = Vector3.Distance(carrycashertr.position, waitTr.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTransform = waitTr;
            }
        }

        return closestTransform;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher")))
        {
            FishCarrydeltime = 0f;
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.GetFishComponentList.Count > 0)
            {
                if (!TargetOtterList.Contains(getvalue))
                {
                    TargetOtterList.Add(getvalue);
                }
            }
        }

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if(getvalue != null)
            {
                if(TargetOtterList.Contains(getvalue))
                {
                    TargetOtterList.Remove(getvalue);
                }
            }
        }
    }


    public override void Update()
    {
        base.Update();

        if (TargetOtterList.Count == 0) return;

        if (FacilityData == null) return;

        if (IsMaxCountCheck()) return;

        for (int i = TargetOtterList.Count -1; i >= 0; i--)
        {
            if (TargetOtterList[i].IsIdle && TargetOtterList[i].GetFishComponentList.Count > 0)
            {
                if (!TargetOtterList[i].IsFishing)
                {
                    FishCarrydeltime += Time.deltaTime;

                    if (FishCarrydeltime >= FishCarryTime)
                    {
                        FishCarrydeltime = 0f;

                        var findfish = TargetOtterList[i].GetFishComponentList.Last();

                        if (findfish != null && findfish.GetFishIdx == FishIdx)
                        {
                            TargetOtterList[i].RemoveFish(findfish);


                            FacilityData.CapacityCountProperty.Value += 1;

                            findfish.FishInBucketAction(FishTrList[FishComponentList.Count], (fish) => {
                                fish.transform.SetParent(this.transform);
                                FishComponentList.Add(findfish);
                            }, 0.2f);

                            if (TargetOtterList[i].GetFishComponentList.Count == 0)
                            {
                                TargetOtterList[i].CarryStart(false);
                                TargetOtterList[i].PlayAnimation(OtterBase.OtterState.Idle, "idle", true);
                            }
                        }

                    }
                }
            }
        }
     
    }
}
