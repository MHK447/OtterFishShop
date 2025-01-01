using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _seedmoney_value;
		public int seedmoney_value
		{
			get { return _seedmoney_value;}
			set { _seedmoney_value = value;}
		}
		[SerializeField]
		private string _stage_name;
		public string stage_name
		{
			get { return _stage_name;}
			set { _stage_name = value;}
		}
		[SerializeField]
		private int _start_consumer_count;
		public int start_consumer_count
		{
			get { return _start_consumer_count;}
			set { _start_consumer_count = value;}
		}

    }

    [System.Serializable]
    public class StageInfo : Table<StageInfoData, int>
    {
    }
}

