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

    private OtterBase Target;

    private InGameStage InGameStage;

    private int FishCount = 0;

    private int FishMaxCount = 0;

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    private float FishPos_Y = 0.15f;

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
            IsOnEnter = true;
            FishCarrydeltime = 0f;
            var getvalue = other.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                Target = getvalue;

                if(FishQueueComponent.Count > 0)
                {
                    Target.IsMoveActive = false;
                }

            }
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

        if (Target.IsIdle && !Target.IsFishing)
        {
            Target.PlayAnimation(OtterBase.OtterState.Carry, "carryIdle", true);
        }

        if (IsOnEnter && !Target.IsFishing)
        {
            FishCarrydeltime += Time.deltaTime;

            if (FishCarrydeltime >= FishCarryTime)
            {
                FishCarrydeltime = 0f;

                var fishcomponent = FishQueueComponent.Dequeue();

                Target.AddFish(fishcomponent);

                var fishcount = Target.GetFishComponentList.Count;

                var posy = Target.GetFishCarryRoot.position.y + (FishPos_Y * (fishcount - 1));

                var fishpos = new Vector3(Target.GetFishCarryRoot.position.x, posy, Target.GetFishCarryRoot.position.z);

                int remainingFish = FishQueueComponent.Count;

                fishcomponent.FishInBucketAction(fishpos, (fish) =>
                {
                    fish.transform.SetParent(Target.GetFishCarryRoot);

                    // 마지막 물고기 작업 완료 시 실행
                    if (remainingFish == 0)
                    {
                        OnAllFishMoved();
                    }
                }, 0.25f);
            }
        }
    }

    // 모든 물고기 이동 후 실행되는 함수
    private void OnAllFishMoved()
    {
        if (Target != null && !Target.IsMoveActive)
        {
            Target.IsMoveActive = true;
            // 추가 작업을 여기서 처리
        }
    }

    public void AddFishQueue(FishComponent fish)
    {
        FishQueueComponent.Enqueue(fish);
    }

}
