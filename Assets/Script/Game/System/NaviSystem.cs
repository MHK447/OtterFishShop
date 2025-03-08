using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviSystem
{
    public enum NaviType
    {
        Counter = 0,
        Rack_01,
        Fish_01,
        CalcCounter,
        UpgradeStart,
        UpgradeBtn,

        End,

    }

    public Queue<NaviType> NaviQueue = new Queue<NaviType>();

    public ArrowNaviUI NaviUI;


    public Dictionary<NaviType ,GameObject> NaviArrowList = new Dictionary<NaviType, GameObject>();
    
    public void Create()
    {
        GameRoot.Instance.UISystem.LoadFloatingUI<ArrowNaviUI>((_naviui) =>
         {
             NaviUI = _naviui;
             ProjectUtility.SetActiveCheck(_naviui.gameObject, false);
         });



    }


    public void FirstStartNavi()
    {
        var recordcount = GameRoot.Instance.UserData.GetRecordCount(Config.RecordCountKeys.Navi_Start);

        if (recordcount == 0)
        {
            GameRoot.Instance.UserData.AddRecordCount(Config.RecordCountKeys.Navi_Start, 1);
            for (int i = 0; i < (int)NaviType.End; ++i)
            {
                NaviQueue.Enqueue((NaviType)i);
            }
        }

    }


    public void StarNexttNavi()
    {
        if (NaviQueue.Count > 0)
        {
            var nextmove = NaviQueue.Dequeue();

            NaviOn(nextmove);
        }
    }


    public void NaviOn(NaviType type)
    {
        var stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
        switch (type)
        {
            case NaviType.Counter:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                    }
                }
                break;
            case NaviType.Rack_01:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.transform);
                    }
                }
                break;
            case NaviType.Fish_01:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.transform);
                    }
                }
                break;
            case NaviType.CalcCounter:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.transform);
                    }
                }
                break;
            case NaviType.UpgradeStart:
                {
                    var getui = GameRoot.Instance.UISystem.GetUI<HUDTotal>();


                }
                break;
            case NaviType.UpgradeBtn:
                {

                }
                break;
        }

    }
}
