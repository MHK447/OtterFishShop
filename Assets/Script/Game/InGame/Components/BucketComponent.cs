using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;

public class BucketComponent : MonoBehaviour
{
    [SerializeField]
    private Transform StartFishTr;

    [SerializeField]
    private Transform AmountUITr;

    private Stack<FishComponent> FishStackComponent = new Stack<FishComponent>();

    public int GetFishCount { get { return FishStackComponent.Count; } }

    private bool IsOnEnter = false;

    private OtterBase Target;

    private InGameStage InGameStage;

    private int FishCount = 0;

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    private float FishPos_Y = 0.15f;

    private FacilityData FacilityData = null;

    private TextCount_UI CountUI = null;

    private CompositeDisposable disposables = new CompositeDisposable();

    private int CapacityMaxCount = 0; 

    public void Init(FacilityData facility)
    {
        FishCount = 0;
        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FacilityData = facility;

        var td = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityData.FacilityIdx);

        if(td != null)
        {
            CapacityMaxCount = td.start_capacity;
        }

        ProjectUtility.SetActiveCheck(this.gameObject, FacilityData.IsOpen);

        GameRoot.Instance.UISystem.LoadFloatingUI<TextCount_UI>((_progress) => {
            CountUI = _progress;
            ProjectUtility.SetActiveCheck(CountUI.gameObject, FacilityData.CapacityCountProperty.Value > 0);
            CountUI.Init(AmountUITr);
            CountUI.SetText(FacilityData.CapacityCountProperty.Value , CapacityMaxCount);
        });

        disposables.Clear();


        FacilityData.CapacityCountProperty.Subscribe(x => {
            if (CountUI != null)
            {
                CountUI.SetText(x, CapacityMaxCount);
            }
        }).AddTo(disposables);

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

    }


    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForSeconds(1f);


        for (int i = 0; i < FacilityData.CapacityCountProperty.Value; ++i)
        {
            var posy = FishPos_Y * (i + 1);

            InGameStage.CreateFish(this.transform, 1, FishComponent.State.Bucket, (fish) => {
                fish.FishInBucketAction(this.transform, (fish) => {
                    FishStackComponent.Push(fish);
                }, 0f, posy);
            });
        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsOnEnter = true;
            FishCarrydeltime = 0f;
            var getvalue = other.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.IsMaxFishCheck() == false)
            {
                Target = getvalue;
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

        if (FishStackComponent.Count <= 0) return;

        if (Target.IsIdle && !Target.IsFishing)
        {
            Target.PlayAnimation(OtterBase.OtterState.Carry, "carryIdle", true);
        }

        if (IsOnEnter && !Target.IsFishing)
        {
            FishCarrydeltime += Time.deltaTime;

            if (FishCarrydeltime >= FishCarryTime && !Target.IsMaxFishCheck())
            {
                FishCarrydeltime = 0f;

                var fishcomponent = FishStackComponent.Pop();

                Target.AddFish(fishcomponent);

                var fishcount = Target.GetFishComponentList.Count;

                var floory = (FishPos_Y * (fishcount - 1));

                int remainingFish = FishStackComponent.Count;

                FacilityData.CapacityCountProperty.Value -= 1;

                fishcomponent.FishInBucketAction(Target.GetFishCarryRoot.transform, (fish) =>
                {
                    fish.transform.SetParent(Target.GetFishCarryRoot);
                }, 0.25f, floory);
            }
        }
    }

    public void AddFishQueue(FishComponent fish)
    {
        FishStackComponent.Push(fish);
        FacilityData.CapacityCountProperty.Value += 1;
    }

}
