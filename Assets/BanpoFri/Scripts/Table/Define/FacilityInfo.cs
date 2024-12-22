using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FacilityInfoData
    {
        [SerializeField]
		private int _facilityidx;
		public int facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}
		[SerializeField]
		private int _initial_count;
		public int initial_count
		{
			get { return _initial_count;}
			set { _initial_count = value;}
		}
		[SerializeField]
		private int _start_capacity;
		public int start_capacity
		{
			get { return _start_capacity;}
			set { _start_capacity = value;}
		}

    }

    [System.Serializable]
    public class FacilityInfo : Table<FacilityInfoData, int>
    {
    }
}

