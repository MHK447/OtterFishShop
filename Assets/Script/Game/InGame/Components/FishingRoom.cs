using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri; 

public class FishingRoom : MonoBehaviour
{
    [SerializeField]
    private BucketComponent BucketComponent;

    private bool IsOnEnter = false;

    public float CurMoneyTime = 0f;

    public float TestTime = 2f;

    private float FishPos_Y = 0.15f;

    private OtterBase Target;

    private InGameStage InGameStage;

    private int FishMaxCount = 0;

    public void Init()
    {
        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CurMoneyTime = 0f;
            IsOnEnter = true;

            var getvalue = other.GetComponent<OtterBase>();

            if (getvalue != null && !getvalue.IsCarry)
                Target = getvalue;

        }
    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
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


        if (Target.IsIdle && !Target.IsFishing && !Target.IsCarry)
        {
            Target.PlayAnimation(OtterBase.OtterState.Fishing, "fishingidle", true);
        }

        if(Target.IsMove)
        {
            CurMoneyTime = 0f;
        }

        if (IsOnEnter && Target.IsFishing)
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
        },1f , posy);
    }
}
