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
        Fishing,
        GoToBucket,
        RackFishAdd,
        CalcCounter,
        WaitCalc,
        UpgradeStart,
        UpgradeBtn,
        End,

    }

    public Queue<NaviType> NaviQueue = new Queue<NaviType>();

    public ArrowNaviUI NaviUI;

    public PopupConversation PopupConversation;

    public Dictionary<NaviType, GameObject> NaviArrowList = new Dictionary<NaviType, GameObject>();


    public List<NaviType> ClearNaviArrowList = new List<NaviType>();


    public NaviType CurNaviOnType = NaviType.End;

    public bool IsNaviOn = false;

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
            IsNaviOn = true;
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
        if (!IsNaviOn) return;

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
        if (NaviUI != null)
        {
            ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
        }

        foreach (var navi in NaviArrowList)
        {
            ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
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
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("해달이의 물고기 왕국! 아니, 가게! 드디어 오픈~! 어… 근데 나 장사하는 법 모르는데? 일단 카운터를 가까이 가서 구매해보자~", PopupConversation.OtterType.Happy), () =>
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
                    GameRoot.Instance.WaitTimeAndCallback(3.5f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("가까이 가서 판매를 할려면 물고기 진열대가 필요해!! 물고기 진열대도 열어보자", PopupConversation.OtterType.Happy));
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.Fish_01:
                {
                    GameRoot.Instance.WaitTimeAndCallback(3.5f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("손님이 몰려왓어!! 언능 낚시하는곳을 오픈해서 손님에게 물고기를 주자!!", PopupConversation.OtterType.Happy));
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 0, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.Fishing:
                {

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing).GetComponent<FishRoomComponent>();

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 1.35f, 0));
                        NaviUI.Init(findfacility.GetCushionComponent.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.GoToBucket:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("좋아!! 양동이 물고기를 손님이 기다리고 있는 진열대로 옮겨보자!!", PopupConversation.OtterType.Happy));

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing).GetComponent<FishRoomComponent>();

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.GetBucketComponent.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.RackFishAdd:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.CalcCounter:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("좋아!! 손님이 가구 있어!! 계산해보자", PopupConversation.OtterType.Happy));

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.UpgradeStart:
                {
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set("좋아!! 벌은 돈으로 시설을 업그레이드 할 수 있어!!", PopupConversation.OtterType.Happy));

                    ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeStart], true);

                }
                break;
            case NaviType.UpgradeBtn:
                {
                    ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeBtn], true);
                }
                break;
        }
    }
}
