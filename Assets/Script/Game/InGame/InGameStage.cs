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
    [SerializeField]
    private InGameBattle Battle;

    public bool IsLoadComplete { get; private set; }

    public InGameBattle GetBattle { get { return Battle; } }

    private CompositeDisposable disposable = new CompositeDisposable();

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        Battle.Init();
    }

            
    public void StartBattle(int stageidx)
    {
       
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
