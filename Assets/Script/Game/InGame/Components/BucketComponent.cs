using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class BucketComponent : MonoBehaviour
{
    [SerializeField]
    private Transform StartFishTr;

    private Queue<FishComponent> FishQueueComponent = new Queue<FishComponent>();

    private bool IsOnEnter = false;

    public float CurMoneyTime = 0f;

    public float TestTime = 2f;

    private float FishPos_Y = 0.15f;

    private OtterBase Target;

    private InGameStage InGameStage;

    private int FishCount = 0;

    private int FishMaxCount = 0;

    public void Init()
    {
        FishCount = 0;
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

            if (getvalue != null)
                Target = getvalue;

        }
    }

    


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(Target != null)
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

        if (FishQueueComponent.Count <= 0) return;


        if(Target.IsIdle && !Target.IsFishing)
        {
            Target.PlayAnimation(OtterBase.OtterState.Carry, "carryidle", true);
        }

        if(IsOnEnter && Target.IsFishing)
        {
            CurMoneyTime += Time.deltaTime;


            var cooltimevalue = (float)CurMoneyTime / (float)TestTime;

            Target.CoolTimeActive(cooltimevalue);

            if (CurMoneyTime >= TestTime)
            {
                CurMoneyTime = 0f;

                var fishcomponent = FishQueueComponent.Dequeue();

                fishcomponent.FishInBucketAction(Target.transform.position, (fish) => {
                    fish.transform.SetParent(Target.transform);
                });

            }
        }
    }



    public void StartFishAction(FishComponent fish)
    {
        FishCount += 1;

        var posy = this.transform.position.y + (FishPos_Y * (FishCount - 1));

        var fishvec = new Vector3(this.transform.position.x, posy, this.transform.position.z);

        fish.FishInBucketAction(fishvec);
    }
}
