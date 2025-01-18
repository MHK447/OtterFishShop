using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FishInfoData
    {
        [SerializeField]
		private int _idx;
		public int idx
		{
			get { return _idx;}
			set { _idx = value;}
		}
		[SerializeField]
		private string _icon;
		public string icon
		{
			get { return _icon;}
			set { _icon = value;}
		}
		[SerializeField]
		private int _base_revenue;
		public int base_revenue
		{
			get { return _base_revenue;}
			set { _base_revenue = value;}
		}

    }

    [System.Serializable]
    public class FishInfo : Table<FishInfoData, int>
    {
    }
}

