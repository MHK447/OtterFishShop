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
        var stageinfotd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx).ToList();

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
        var tdstagelist = Tables.Instance.GetTable<ConsumerMoveInfo>().DataList.ToList().FindAll(x=> x.stageidx == stageidx); // facility 안열린것도 포함시키기 

        List<ConsumerMoveInfoData> patternlist = new List<ConsumerMoveInfoData>();

        for (int i = 0; i < tdstagelist.Count; i++)
        {
            // 모든 시설이 조건을 만족하면 true, 하나라도 만족하지 않으면 false
            bool allFound = true;

            for (int j = 0; j < tdstagelist[i].facilityidx.Count; j++)
            {
                var finddata = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.FindFacility(tdstagelist[i].facilityidx[j]);

                if(finddata == null)
                {
                    allFound = false;
                    break;
                }

                if (!finddata.IsOpenFacility())
                {
                    allFound = false;
                    break;
                }
            }

            // 모든 facilityidx가 조건을 만족했다면 patternlist에 추가
            if (allFound)
            {
                patternlist.Add(tdstagelist[i]);
            }
        }


        if(patternlist.Count > 0 )
        {
            var randvalue = Random.Range(0, patternlist.Count);


            return patternlist[randvalue];
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
