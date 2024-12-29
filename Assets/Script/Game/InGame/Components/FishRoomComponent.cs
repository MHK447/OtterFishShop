using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class FishRoomComponent : FacilityComponent
{
    [SerializeField]
    private FishCushionComponent CushionComponent;

    [SerializeField]
    private BucketComponent BucketComponent;


    public FishCushionComponent GetCushionComponent { get { return CushionComponent; } }


    public BucketComponent GetBucketComponent { get { return BucketComponent; } }


    public override void Init()
    {
        base.Init();

        ProjectUtility.SetActiveCheck(CushionComponent.gameObject, false);
        ProjectUtility.SetActiveCheck(BucketComponent.gameObject, false);

        var finddata = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);

        if(finddata != null)
        {
            CushionComponent.Init(finddata);
            BucketComponent.Init(finddata);

        }

        Col.enabled = !FacilityData.IsOpen;
    }
}
