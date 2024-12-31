using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;

public class FishCushionComponent : MonoBehaviour
{
    [SerializeField]
    private BucketComponent BucketComponent;

    private bool IsOnEnter = false;

    public float CurMoneyTime = 0f;

    public float TestTime = 2f;

    private float FishPos_Y = 0.15f;

    private OtterBase Target;

    private InGameStage InGameStage;

    private FacilityData FacilityData = null;

    private int CapacityMaxCount = 0;

    public void Init(FacilityData facility)
    {
        FacilityData = facility;

        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        var td = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityData.FacilityIdx);

        if(td != null)
        {
            CapacityMaxCount = td.start_capacity;
        }

        ProjectUtility.SetActiveCheck(this.gameObject, FacilityData.IsOpen);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FacilityData.CapacityCountProperty.Value >= CapacityMaxCount) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && InGameStage.FindCasher(CasherType.FishingCasher, FacilityData.FacilityIdx) != null) return;


        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            CurMoneyTime = 0f;
            IsOnEnter = true;

            var getvalue = other.GetComponent<OtterBase>();

            if (getvalue != null)
            {

                if (getvalue != null && !getvalue.IsCarry)
                    Target = getvalue;
            }

        }
    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && InGameStage.FindCasher(CasherType.FishingCasher, FacilityData.FacilityIdx) != null) return;


        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            if (Target != null)
            {
                Target.CoolTimeActive(0f);
            }

            Target = null;
            IsOnEnter = false;
        }
    }


    private void Update()
    {
        if (Target == null) return;

        if (FacilityData == null) return;

        if (FacilityData.IsOpen == false) return;

        if (FacilityData.CapacityCountProperty.Value >= CapacityMaxCount) return;

        if (Target.IsIdle && !Target.IsFishing && !Target.IsCarry)
        {
            Target.PlayAnimation(OtterBase.OtterState.Fishing, "fishingidle", true);
        }

        if(Target.IsMove)
        {
            CurMoneyTime = 0f;
        }

        if (IsOnEnter && Target.IsFishing && FacilityData.CapacityCountProperty.Value < CapacityMaxCount)
        {
            CurMoneyTime += Time.deltaTime;


            var cooltimevalue = (float)CurMoneyTime / (float)TestTime;

            Target.CoolTimeActive(cooltimevalue);

            if (CurMoneyTime >= TestTime)
            {
                CurMoneyTime = 0f;

                InGameStage.CreateFish(Target.GetFishTr, 1, FishComponent.State.Bucket, StartFishAction);

            }
        }
    }



    public void StartFishAction(FishComponent fish)
    {
        var fishcount = BucketComponent.GetFishCount;
        var posy = FishPos_Y * fishcount;

        fish.FishInBucketAction(BucketComponent.transform, (fish)=> {
            BucketComponent.AddFishQueue(fish);

            Target.CoolTimeActive(FacilityData.CapacityCountProperty.Value < CapacityMaxCount);
        },1f , posy);
    }
}
