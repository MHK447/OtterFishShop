using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class FacilitySystem 
{
    private int[] OpenFacilityDatas = { 1, 1000 };

    public ConsumerMoveInfoData CreatePattern(int stageidx)
    {
        var tdstagelist = Tables.Instance.GetTable<ConsumerMoveInfo>().DataList.ToList().FindAll(x=> x.stageidx == stageidx && IsOpenPattern(x.facilityidx)); // facility 안열린것도 포함시키기 


        List<int> patternlist = new List<int>();

        if(tdstagelist.Count > 0)
        {
            var randvalue = Random.Range(0, tdstagelist.Count);

            return tdstagelist[randvalue];
        }

        return null;
    }



    public bool IsOpenPattern(List<int> facilityidxlist)
    {
        bool isopen = true;


        foreach(var facilityidx in facilityidxlist)
        {
            if(OpenFacilityDatas.Contains(facilityidx) == false)
            {
                isopen = false;

                break;
            }
        }

        return isopen;
    }
}
