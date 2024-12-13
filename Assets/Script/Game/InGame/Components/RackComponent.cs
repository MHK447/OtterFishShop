using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class RackComponent : FacilityComponent
{
    [SerializeField]
    private int FishIdx = 0;

    [SerializeField]
    private List<Transform> FishTrList = new List<Transform>();

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    [SerializeField]
    private SpriteRenderer FishIcon;

    private OtterBase Target;

    private bool IsOnEnter = false;

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    public override void Init(int facilityidx)
    {
        base.Init(facilityidx);

        var td = Tables.Instance.GetTable<FishInfo>().GetData(FishIdx);

        if(td != null)
        {
            FishIcon.sprite = Config.Instance.GetIngameImg(td.icon);
        }

        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsOnEnter = true;
            FishCarrydeltime = 0f;
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
                Target = getvalue;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
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

        if (Target.IsIdle && Target.GetFishComponentList.Count > 0)
        {

            if (IsOnEnter && !Target.IsFishing)
            {
                FishCarrydeltime += Time.deltaTime;

                if (FishCarrydeltime >= FishCarryTime)
                {
                    FishCarrydeltime = 0f;

                    var findfish = Target.GetFishComponentList.Last();

                    if(findfish != null && findfish.GetFishIdx == FishIdx)
                    {
                        Target.GetFishComponentList.Remove(findfish);

                        FishComponentList.Add(findfish);

                        findfish.FishInBucketAction(FishTrList[FishComponentList.Count - 1].position, (fish)=> { fish.transform.SetParent(this.transform); }, 0.2f);


                        if(Target.GetFishComponentList.Count == 0)
                        {
                            Target.CarryStart(false);
                            Target.PlayAnimation(OtterBase.OtterState.Idle, "idle", true);
                        }
                    }

                }
            }
        }
    }
}
