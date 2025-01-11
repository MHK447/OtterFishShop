using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class CookingInfoData
    {
        [SerializeField]
		private int _cookingidx;
		public int cookingidx
		{
			get { return _cookingidx;}
			set { _cookingidx = value;}
		}
		[SerializeField]
		private List<int> _material_idxs;
		public List<int> material_idxs
		{
			get { return _material_idxs;}
			set { _material_idxs = value;}
		}
		[SerializeField]
		private int _material_max_count;
		public int material_max_count
		{
			get { return _material_max_count;}
			set { _material_max_count = value;}
		}
		[SerializeField]
		private int _Food_idx;
		public int Food_idx
		{
			get { return _Food_idx;}
			set { _Food_idx = value;}
		}

    }

    [System.Serializable]
    public class CookingInfo : Table<CookingInfoData, int>
    {
    }
}

