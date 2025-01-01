using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Linq;
using BanpoFri;

public enum CasherType
{
    CounterCasher = 1,
    CarryCasher = 2,
    FishingCasher = 3,
}

public class InGameStage : MonoBehaviour
{
    public bool IsLoadComplete { get; private set; }

    private CompositeDisposable disposable = new CompositeDisposable();

    [SerializeField]
    private List<Transform> WaitLineListTr = new List<Transform>();

    [SerializeField]
    private List<Transform> StartWayPointTrList = new List<Transform>();

    [SerializeField]
    private Transform EndTr;

    public Transform GetEndTr { get { return EndTr; } }

    [SerializeField]
    private Transform MiddleEndTr;

    public Transform GetMiddleEndTr { get { return MiddleEndTr; } }

    [SerializeField]
    private AssetReference FishRef;

    [SerializeField]
    private AssetReference ConsumerRef;

    [SerializeField]
    private List<FishRoomComponent> FishRoomList = new List<FishRoomComponent>();

    [SerializeField]
    private List<FacilityComponent> FacilityList = new List<FacilityComponent>();

    [SerializeField]
    private CounterComponent CounterComponent;

    public CounterComponent GetCounterComponent { get { return CounterComponent; } }

    public Transform GetStartWayPoint { get { return StartWayPointTrList[0]; } }

    private ObjectPool<FishComponent> FishPool = new ObjectPool<FishComponent>();

    private ObjectPool<Consumer> ConsumerPool = new ObjectPool<Consumer>();

    private List<FishComponent> activeFishObjs = new List<FishComponent>();

    private List<OtterBase> activeCashers = new List<OtterBase>();

    private List<Consumer> activeConsumerObjs = new List<Consumer>();

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        FishPool.Init(FishRef, this.transform ,30);
        ConsumerPool.Init(ConsumerRef, this.transform, 10);
        CreatePoolCasher(10);



        foreach (var facility in FacilityList)
        {
            facility.Init();
        }




        foreach(var fishroom in FishRoomList)
        {
            fishroom.Init();
        }


        GameRoot.Instance.WaitTimeAndCallback(2f, () => {
            GameRoot.Instance.UpgradeSystem.StartUpgradeCheck();

            var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.AddCustomer);

            for (int i = 0; i < upgradevalue; ++i)
            {
                CreateConsumer(1, StartWayPointTrList[i]);
            }
        });
    }

            
    public void ReturnMainScreen()
    {
        GameRoot.Instance.UserData.CurMode.GachaCoin.Value = 0;
        GameRoot.Instance.UserData.CurMode.StageData.IsStartBattle = false;
    }

    private void OnDestroy()
    {
        disposable.Clear();
    }


    public void CreateFish(Transform starttr , int fishidx , FishComponent.State state , System.Action<FishComponent> fishcallback = null)
    {
        FishPool.Get((obj) => {
            obj.transform.position = starttr.position;
            activeFishObjs.Add(obj);
            obj.OnEnd += (complete) => {

                FishPool.Return(obj);
                activeFishObjs.Remove(obj);
            };

            obj.Set(fishidx , state);

            fishcallback?.Invoke(obj);
        });
    }


    public void CreateConsumer(int consumeridx , Transform starttr,  System.Action<Consumer> consumercallback = null)
    {
        ConsumerPool.Get((obj) => {
            obj.transform.position = starttr.position;
            activeConsumerObjs.Add(obj);
            obj.OnEnd += (complete) => {

                ConsumerPool.Return(obj);
                activeConsumerObjs.Remove(obj);
                CreateConsumer(1, starttr);
            };

            obj.Init(consumeridx);

            consumercallback?.Invoke(obj);
        });
    }


    public void CreatePoolCasher(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            Addressables.InstantiateAsync("CarryCasher").Completed += (handle) => {
                var casher = handle.Result.GetComponent<CarryCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };

            Addressables.InstantiateAsync("FishCasher").Completed += (handle) => {
                var casher = handle.Result.GetComponent<FishCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };

            Addressables.InstantiateAsync("CounterCasher").Completed += (handle) => {
                var casher = handle.Result.GetComponent<CounterCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };
        }
    }

    public OtterBase ActiveCarryCasher(CasherType type)
    {
        var finddata = activeCashers.Find(x => x.gameObject.activeSelf == false && x.GetCasherIdx == (int)type);

        if(finddata != null)
        {
            return finddata;
        }


        return null;
    }

    public OtterBase FindCasher(CasherType type , int facilityidx)
    {
        OtterBase casher = null;

        switch(type)
        {
            case CasherType.FishingCasher:
                {
                    var findcashers = activeCashers.FindAll(x => x.GetCasherIdx == (int)type && x.gameObject.activeSelf);


                    foreach(var fishcasher in findcashers)
                    {
                        if(fishcasher.GetComponent<FishCasher>().GetFacilityIdx == facilityidx)
                        {
                            casher = fishcasher;
                        }
                    }
                }
                break;
        }

        return casher;
    }



    public Transform GetWaitLine(int order)
    {
        if(WaitLineListTr.Count > order)
        {
            return WaitLineListTr[order];
        }

        return WaitLineListTr.First();
    }


    public Transform GetFacilityConsumeTr(int facilityidx)
    {
        var finddata = FacilityList.Find(x => x.FacilityIdx == facilityidx);

        if(finddata != null)
        {
            return finddata.GetConsumerTr();
        }

        return null;
    }



    public FacilityComponent FindFacility(int facilityidx)
    {
        var finddata = FacilityList.Find(x => x.FacilityIdx == facilityidx);

        if (finddata != null)
        {
            return finddata;
        }

        return null;
    }
}
