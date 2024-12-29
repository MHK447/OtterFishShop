using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class FacilitySystem 
{
    public enum FacilityType
    {
        FishDisplay = 1,
        Counter = 2,
        Fishing = 3,
        Cooked = 4,
      
    }


    public void Create()
    {
    }

    public void CreateStageFacility(int stageidx)
    {
        var stageinfotd = Tables.Instance.GetTable<StageInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx).ToList();

        GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Clear();

        foreach (var stageinfo in stageinfotd)
        {
            var newfacility = new FacilityData(stageinfo.facilityidx, 0, false , 0);

            GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Add(newfacility);
        }

        GameRoot.Instance.UserData.Save();
    }


    public ConsumerMoveInfoData CreatePattern(int stageidx)
    {
        var tdstagelist = Tables.Instance.GetTable<ConsumerMoveInfo>().DataList.ToList().FindAll(x=> x.stageidx == stageidx ); // facility 안열린것도 포함시키기 

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
            var facilitydata = GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Find(x => x.FacilityIdx == facilityidx);
            if (facilitydata != null && facilitydata.IsOpen == false)
            {
                isopen = false;

                break;
            }
        }

        return isopen;
    }
}
