using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FacilityOpenOrderData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _openorder;
		public int openorder
		{
			get { return _openorder;}
			set { _openorder = value;}
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
    public class FacilityOpenOrder : Table<FacilityOpenOrderData, KeyValuePair<int,int>>
    {
    }
}

