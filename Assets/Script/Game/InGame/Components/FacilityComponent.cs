using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;
using System.Linq;

public class FacilityComponent : MonoBehaviour
{
    [SerializeField]
    protected BoxCollider2D RigidCol;
    
    [SerializeField]
    private List<GameObject> FacilityOpenList = new List<GameObject>();

    [SerializeField]
    protected List<Transform> ConsumerWaitTr = new List<Transform>();

    [SerializeField]
    private ContentsOpenComponent ContentsOpenComponent;

    public int ConsumerOrder = 0;

    public int FacilityIdx = 0;

    protected int CapacityMaxCount = 0;

    protected FacilityData FacilityData;

    public FacilityData GetFacilityData { get { return FacilityData; } }

    private CompositeDisposable disposables = new CompositeDisposable();

    protected InGameStage InGameStage;

    protected OtterBase Player;

    protected int BaseCapacity = 0;
    public virtual void Init()
    {
        ConsumerOrder = 0;

        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityIdx);

        BaseCapacity = facilitytd.start_capacity;

        var stagefacilitytd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.Where(x => x.facilityidx == FacilityIdx
        && x.stageidx == stageidx).FirstOrDefault();

        if (stagefacilitytd == null) return;

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.ShelfCapacityUp,FacilityIdx);

        CapacityMaxCount = BaseCapacity + (int)buffvalue;

        RigidCol.isTrigger = !FacilityData.IsOpen;
      
        foreach (var facility in FacilityOpenList)
        {
            ProjectUtility.SetActiveCheck(facility, FacilityData.IsOpen);
        }

        ContentsOpenComponent.Set(FacilityData, OpenFacility);
    }

    public virtual Transform GetConsumerTr()
    {
        if (ConsumerWaitTr.Count == 0) return null; 

        var randvalue = Random.Range(0, ConsumerWaitTr.Count);
        return ConsumerWaitTr[randvalue];

    }

    


    public virtual bool IsMaxCountCheck()
    {
        if (FacilityData == null) return false;


        return FacilityData.CapacityCountProperty.Value >= CapacityMaxCount;
    }
    

    public Transform GetConsumerTr(int order)
    {
        return ConsumerWaitTr[order];
    }

    public bool IsOpenFacility()
    {
        if (FacilityData == null) return false;


        return FacilityData.IsOpen;
    }



    public void OpenFacility()
    {
        FacilityData.IsOpen = true;
        GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty.Value += 1;
        Init();

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var stageinfotd = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        if (stageinfotd != null)
        {
            if (stageinfotd.consumerfirst_idx == FacilityIdx)
            {
                var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.AddCustomer);

                for (int i = 0; i < upgradevalue; ++i)
                {
                    InGameStage.CreateConsumer(1, InGameStage.GetStartWayPoint);
                }
            }
        }

    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

}
