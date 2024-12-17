using System;
using System.Numerics;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public interface IReadOnlyData : ICloneable { 
	void Create();
}
public interface IClientData { }


public class FacilityData
{
	public bool IsOpen = false;

	public int FacilityIdx = 0;

	public int MoneyCount = 0;

	public FacilityData(int facilityidx , int moneycount , bool isopen)
    {
		IsOpen = isopen;
		FacilityIdx = facilityidx;
		MoneyCount = moneycount;
    }

}

public class StageData
{
	public List<FacilityData> StageFacilityDataList = new List<FacilityData>();

	public int StageIdx { get; set; } = 1;
	public IReactiveProperty<int> WaveIdxProperty = new ReactiveProperty<int>();
	public IReactiveProperty<int> UnitCountPropety = new ReactiveProperty<int>();
	public IReactiveProperty<int> WaveRewardProperty = new ReactiveProperty<int>();

	public IReactiveProperty<int> WaveTimeProperty = new ReactiveProperty<int>();

	public int UnitAddCount = 0;

	public int StageHighWave = 1;

	public int SelectSkill = 0;

	public bool IsStartBattle = false;

	public IReactiveProperty<bool> IsBossProperty = new ReactiveProperty<bool>(false);


	public FacilityData FindFacilityData(int facilityidx)
    {
		return StageFacilityDataList.Find(x => x.FacilityIdx == facilityidx);
    }

	public void StageEndClear()
    {
		WaveIdxProperty.Value = 1;
		UnitAddCount = 0;
		UnitCountPropety.Value = 0;
		WaveTimeProperty.Value = 0;

	}


	public void SetStage(int stageidx)
	{
		GameRoot.Instance.FacilitySystem.CreateStageFacility(GameRoot.Instance.UserData.CurMode.StageData.StageIdx);
		StageIdx = stageidx;
    }

	public void SetWave(int waveidx)
    {
		WaveIdxProperty.Value = waveidx;	
    }
}





