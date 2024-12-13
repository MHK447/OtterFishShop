using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class FacilitySystem 
{


    public ConsumerMoveInfoData CreatePattern(int stageidx)
    {
        var tdstagelist = Tables.Instance.GetTable<ConsumerMoveInfo>().DataList.ToList().FindAll(x=> x.stageidx == stageidx); // facility 안열린것도 포함시키기 


        List<int> patternlist = new List<int>();

        if(tdstagelist.Count > 0)
        {
            var randvalue = Random.Range(0, tdstagelist.Count);

            return tdstagelist[randvalue];
        }


        return null;
    }
}
