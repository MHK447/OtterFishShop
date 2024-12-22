using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UniRx;

public class RackComponent : FacilityComponent
{
    [SerializeField]
    private int FishIdx = 0;

    [SerializeField]
    private List<Transform> FishTrList = new List<Transform>();

    [SerializeField]
    private Transform AmountUITr;

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    public List<FishComponent> GetFishComponentList { get { return FishComponentList; } }

    [SerializeField]
    private SpriteRenderer FishIcon;

    private OtterBase Target;

    private bool IsOnEnter = false;

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    private UI_AmountBubble AmountUI = null;

    private CompositeDisposable disposables = new CompositeDisposable();

    public override void Init()
    {
        base.Init();

        var td = Tables.Instance.GetTable<FishInfo>().GetData(FishIdx);

        if(td != null)
        {
            FishIcon.sprite = Config.Instance.GetIngameImg(td.icon);
        }

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(FacilityIdx);


        GameRoot.Instance.UISystem.LoadFloatingUI<UI_AmountBubble>((_progress) => {
            AmountUI = _progress;
            ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.CapacityCountProperty.Value > 0);
            AmountUI.Init(AmountUITr);
            AmountUI.Set(FacilityData.FacilityIdx);
            AmountUI.SetValue(FacilityData.CapacityCountProperty.Value);
        });


        FacilityData.CapacityCountProperty.Subscribe(x => {
            if (AmountUI != null)
            {   
                AmountUI.SetValue(x);
            }
        }).AddTo(disposables);


        GameRoot.Instance.StartCoroutine(WaitOneFrame());
    }


    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < FacilityData.CapacityCountProperty.Value; ++i)
        {
            InGameStage.CreateFish(this.transform, 1, FishComponent.State.Rack, (fish) => {
                fish.FishInBucketAction(FishTrList[i], (fish) => {
                    fish.transform.SetParent(this.transform);
                    FishComponentList.Add(fish);
                }, 0f);
            });
        }


    }

    public void RemoveFish()
    {
        var target = FishComponentList.First();

        FishComponentList.Remove(target);

        FacilityData.CapacityCountProperty.Value -= 1;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !IsMaxCountCheck())
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


    public override void Update()
    {
        base.Update();

        if (Target == null) return;

        if (FacilityData == null) return;

        if (IsMaxCountCheck()) return;

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

                        FacilityData.CapacityCountProperty.Value += 1;

                        findfish.FishInBucketAction(FishTrList[FishComponentList.Count], (fish)=> {
                            fish.transform.SetParent(this.transform);
                            FishComponentList.Add(findfish);
                        }, 0.2f);

                        if (Target.GetFishComponentList.Count == 0)
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
