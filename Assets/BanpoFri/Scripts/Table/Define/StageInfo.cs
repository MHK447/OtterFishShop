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
		private int _facilityidx;
		public int facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}

    }

    [System.Serializable]
    public class StageInfo : Table<StageInfoData, KeyValuePair<int,int>>
    {
    }
}

