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


public class StageData
{
	public int StageIdx { get; set; } = 0;
	public IReactiveProperty<int> WaveIdxProperty = new ReactiveProperty<int>();
	public IReactiveProperty<int> UnitCountPropety = new ReactiveProperty<int>();
	public IReactiveProperty<int> WaveRewardProperty = new ReactiveProperty<int>();

	public IReactiveProperty<int> WaveTimeProperty = new ReactiveProperty<int>();

	public int UnitAddCount = 0;

	public int StageHighWave = 1;

	public int SelectSkill = 0;

	public bool IsStartBattle = false;

	public IReactiveProperty<bool> IsBossProperty = new ReactiveProperty<bool>(false);

	public void StageEndClear()
    {
		WaveIdxProperty.Value = 1;
		UnitAddCount = 0;
		UnitCountPropety.Value = 0;
		WaveTimeProperty.Value = 0;

	}


	public void SetStage(int stageidx)
    {
		StageIdx = stageidx;
    }

	public void SetWave(int waveidx)
    {
		WaveIdxProperty.Value = waveidx;	
    }
}





