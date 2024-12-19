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
	public IReactiveProperty<int> NextFacilityOpenOrderProperty = new ReactiveProperty<int>();
	public bool IsStartBattle = false;


	public FacilityData FindFacilityData(int facilityidx)
    {
		return StageFacilityDataList.Find(x => x.FacilityIdx == facilityidx);
    }

	public void StageEndClear()
    {

	}


	public void SetStage(int stageidx)
	{
		GameRoot.Instance.FacilitySystem.CreateStageFacility(GameRoot.Instance.UserData.CurMode.StageData.StageIdx);
		StageIdx = stageidx;
    }

	public void SetWave(int waveidx)
    {
    }
}





