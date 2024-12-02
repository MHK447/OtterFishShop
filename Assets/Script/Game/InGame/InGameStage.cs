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
    private Transform StartWayPointTr;

    [SerializeField]
    private Transform EndTrTest;

    public Transform GetStartWayPoint { get { return StartWayPointTr; } }

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();


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


                    getobj.Init(1, EndTrTest);
                }

            };
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
}
