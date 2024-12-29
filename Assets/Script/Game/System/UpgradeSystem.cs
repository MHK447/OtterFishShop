using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;


public class UpgradeSystem
{
    public enum UpgradeType
    {
        AddEmployee = 1,          // 직원 추가
        IncreaseEmployeeSpeed = 2, // 직원 속도 업
        AddCustomer = 3,          // 손님 추가
        IncreasePlayerSpeed = 4,  // 플레이어 속도 업
        IncreasePlayerCapacity = 5, // 플레이어 용량 추가
        IncreaseShelfCapacity = 6,  // 매대 용량 추가
        IncreaseCookingSpeed = 7, // 음식 조리 속도 업
        IncreaseEmployeeCapacity = 8 // 직원 용량 추가
    }


    public void AddUpgradeData(int upgradetype)
    {
        switch(upgradetype)
        {
            case (int)UpgradeType.AddEmployee:
                {

                    var ingamestage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();
                    var finddata = ingamestage.curInGameStage.GetCarryCasher();

                    if(finddata != null)
                    {
                        finddata.transform.position = ProjectUtility.GetRandomPositionAroundTarget(ingamestage.GetPlayer.transform.position, 2f);
                        ProjectUtility.SetActiveCheck(finddata.gameObject, true);
                        finddata.Init(1);
                    }
                }
                break;
            case (int)UpgradeType.IncreaseEmployeeSpeed:
                break;
            case (int)UpgradeType.AddCustomer:
                break;
            case (int)UpgradeType.IncreasePlayerSpeed:
                break;
            case (int)UpgradeType.IncreasePlayerCapacity:
                break;
            case (int)UpgradeType.IncreaseShelfCapacity:
                break;
            case (int)UpgradeType.IncreaseCookingSpeed:
                break;
            case (int)UpgradeType.IncreaseEmployeeCapacity:
                break;
        }
    }

    public void StageSetUpgradeData(int stageidx)
    {

        var finddata = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().Find(x => x.StageIdx == stageidx);

        if(finddata == null)
        {
            GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.Clear();


            var upgradelist = Tables.Instance.GetTable<UpgradeInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx);

            foreach(var upgrade in upgradelist)
            {
                var newdata = new UpgradeData(upgrade.upgrade_idx, upgrade.upgrade_type, stageidx, false);

                GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.Add(newdata);
            }

        }

        GameRoot.Instance.UserData.Save();
    }
}
