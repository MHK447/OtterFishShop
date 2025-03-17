using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;

public class NaviSystem
{
    public enum NaviType
    {
        Conversation_1,
        Counter,
        Rack_01,
        Fish_01,
        CalcCounter,
        UpgradeBtn,
        UpgradeStart,
        End,

    }

    public Queue<NaviType> NaviQueue = new Queue<NaviType>();

    public ArrowNaviUI NaviUI;

    public PopupConversation PopupConversation;

    public Dictionary<NaviType, GameObject> NaviArrowList = new Dictionary<NaviType, GameObject>();


    public List<NaviType> ClearNaviArrowList = new List<NaviType>();


    public NaviType CurNaviOnType = NaviType.End;

    public void Create()
    {
        GameRoot.Instance.UISystem.LoadFloatingUI<ArrowNaviUI>((_naviui) =>
         {
             NaviUI = _naviui;
             ProjectUtility.SetActiveCheck(_naviui.gameObject, false);
         });

        GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Hide());
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

    public void NextNavi(NaviType type)
    {

        if (!ClearNaviArrowList.Contains(type))
        {
            ClearNaviArrowList.Add(type);
            if (NaviUI != null)
            {
                ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
            }

            foreach (var navi in NaviArrowList)
            {
                ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
            }
            StarNexttNavi();
        }
    }

    public void NaviOff(NaviType naviontype)
    {
        if (naviontype == CurNaviOnType)
        {
            if (NaviUI != null)
            {
                ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
            }

            foreach (var navi in NaviArrowList)
            {
                ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
            }
        }
    }


    public void NaviOn(NaviType type)
    {
        if (NaviUI != null)
        {
            ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
        }

        foreach (var navi in NaviArrowList)
        {
            ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
        }

        var stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
        switch (type)
        {
            case NaviType.Conversation_1:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("해달이 테스트 스껄스껄", PopupConversation.OtterType.Happy), () =>
                    {
                        CurNaviOnType = NaviType.Counter;
                        NextNavi(CurNaviOnType);
                    });
                }
                break;
            case NaviType.Counter:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.Rack_01:
                {
                    GameRoot.Instance.WaitTimeAndCallback(4f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("가까이 가서 판매를 할려면 물고기 진열대가 필요해!! 물고기 진열대도 열어보자", PopupConversation.OtterType.Happy));
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.Fish_01:
                {
                    GameRoot.Instance.WaitTimeAndCallback(4f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("손님이 몰려왓어!! 언능 낚시하는곳을 오픈해서 손님에게 물고기를 주자!!", PopupConversation.OtterType.Happy));
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.CalcCounter:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(-0.9f, 2f, 0));
                        NaviUI.Init(findfacility.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.UpgradeStart:
                {
                    if (NaviArrowList.ContainsKey((NaviType.UpgradeStart)))
                    {
                        ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeStart], true);
                    }
                }
                break;
            case NaviType.UpgradeBtn:
                {
                    if (NaviArrowList.ContainsKey((NaviType.UpgradeBtn)))
                    {
                        ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeBtn], true);
                    }
                }
                break;
        }
    }
}
