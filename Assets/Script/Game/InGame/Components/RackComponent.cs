using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UniRx;

public class RackComponent : FacilityComponent
{

    protected IReactiveCollection<FishComponent> FishComponentList = new ReactiveCollection<FishComponent>();

    public List<FishComponent> GetFishComponentList { get { return FishComponentList.ToList(); } }

    [SerializeField]
    private int FishIdx = 0;

    [SerializeField]
    private List<Transform> FishTrList = new List<Transform>();

    [SerializeField]
    private Transform AmountUITr;

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

        FacilityData.CapacityCountProperty.Value = 0;

        GameRoot.Instance.UISystem.LoadFloatingUI<UI_AmountBubble>((_progress) => {
            AmountUI = _progress;
            ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.IsOpen);
            //ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.CapacityCountProperty.Value > 0);
            AmountUI.Init(AmountUITr);
            AmountUI.Set(FacilityData.FacilityIdx);
            AmountUI.SetValue(FacilityData.CapacityCountProperty.Value,CapacityMaxCount);
        });

        disposables.Clear();

        FishComponentList.ObserveAdd().Subscribe(x => {
            if (AmountUI != null)
            {   
                AmountUI.SetValue(FishComponentList.Count , CapacityMaxCount);
            }
        }).AddTo(disposables);

        FishComponentList.ObserveRemove().Subscribe(x => {
            if (AmountUI != null)
            {
                AmountUI.SetValue(FishComponentList.Count , CapacityMaxCount);
            }
        }).AddTo(disposables);


        var donebuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);

        foreach (var donebuy in donebuylist)
        {
            donebuy.IsBuyCheckProperty.Subscribe(x => {
                if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.ShelfCapacityUp)
                {
                    var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

                    var upgradetd = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, donebuy.UpgradeIdx));

                    if (upgradetd != null && upgradetd.value2 == FacilityIdx)
                    {
                        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.ShelfCapacityUp, FacilityIdx);

                        CapacityMaxCount = BaseCapacity + (int)buffvalue;

                        if(AmountUI != null)
                            AmountUI.SetValue(FacilityData.CapacityCountProperty.Value, CapacityMaxCount);
                    }
                }

            }).AddTo(disposables);
        }

        if(AmountUI != null)
            ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.IsOpen);

    }

    public void RemoveFish()
    {
        var target = FishComponentList.Last();

        FishComponentList.Remove(target);

        FacilityData.CapacityCountProperty.Value -= 1;

        if (AmountUI != null)
            AmountUI.SetValue(FishComponentList.Count, CapacityMaxCount);
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

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
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
                                TargetOtterList[i].CarryEnd();
                            }
                        }

                    }
                }
            }
        }
     
    }
}
