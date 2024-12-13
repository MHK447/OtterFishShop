using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Linq;
using BanpoFri;

public class InGameStage : MonoBehaviour
{
    public bool IsLoadComplete { get; private set; }

    private CompositeDisposable disposable = new CompositeDisposable();

    [SerializeField]
    private List<Transform> WaitLineListTr = new List<Transform>();

    [SerializeField]
    private Transform StartWayPointTr;

    [SerializeField]
    private Transform EndTrTest;

    [SerializeField]
    private AssetReference FishRef;

    [SerializeField]
    private List<FishingRoom> FishingRoomList = new List<FishingRoom>();

    [SerializeField]
    private List<BucketComponent> BucketList = new List<BucketComponent>();

    [SerializeField]
    private List<FacilityComponent> FacilityList = new List<FacilityComponent>();

    public Transform GetStartWayPoint { get { return StartWayPointTr; } }

    private ObjectPool<FishComponent> FishPool = new ObjectPool<FishComponent>();

    private List<FishComponent> activeObjs = new List<FishComponent>();

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        FishPool.Init(FishRef, this.transform ,30);

        //test
        var td = Tables.Instance.GetTable<ConsumerInfo>().GetData(1);

        if (td != null)
        {
            Addressables.InstantiateAsync(td.prefab).Completed += (handle) =>
            {

                var getobj = handle.Result.GetComponent<Consumer>();

                if(getobj != null)
                {
                    ProjectUtility.SetActiveCheck(handle.Result.gameObject, true);

                    getobj.transform.position = StartWayPointTr.position;


                    getobj.Init(1);
                }

            };
        }


        foreach(var fishingroom in FishingRoomList)
        {
            fishingroom.Init();
        }

        foreach(var bucket in BucketList)
        {
            bucket.Init();
        }
    }

            
    public void ReturnMainScreen()
    {
        GameRoot.Instance.UserData.CurMode.StageData.SelectSkill = 0;
        GameRoot.Instance.UserData.CurMode.GachaCoin.Value = 0;
        GameRoot.Instance.UserData.CurMode.StageData.WaveRewardProperty.Value = 0;
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
            activeObjs.Add(obj);
            obj.OnEnd += (complete) => {

                FishPool.Return(obj);
                activeObjs.Remove(obj);
            };

            obj.Set(fishidx , state);

            fishcallback?.Invoke(obj);
        });
    }


    public Transform GetWaitLine(int order)
    {
        if(WaitLineListTr.Count > order)
        {
            return WaitLineListTr[order];
        }

        return WaitLineListTr.First();
    }


    public Transform GetFacilityTr(int facilityidx)
    {
        var finddata = FacilityList.Find(x => x.FacilityIdx == facilityidx);

        if(finddata != null)
        {
            return finddata.GetConsumerTr();
        }

        return null;
    }
}
