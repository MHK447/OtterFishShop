using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class BucketComponent : MonoBehaviour
{
    [SerializeField]
    private Transform StartFishTr;

    private bool IsOnEnter = false;

    public float CurMoneyTime = 0f;

    public float TestTime = 2f;

    private OtterBase Target;

    private InGameStage InGameStage;

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


            if(getvalue != null)
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

        if(IsOnEnter)
        {
            CurMoneyTime += Time.deltaTime;


            var cooltimevalue = (float)CurMoneyTime / (float)TestTime;

            Target.CoolTimeActive(cooltimevalue);

            if (CurMoneyTime >= TestTime)
            {
                CurMoneyTime = 0f;


                InGameStage.CreateFish(StartFishTr, 1 , FishComponent.State.Bucket, StartFishAction);
                


                //GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, 10);


                //GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMoney>(new Vector3(Target.transform.position.x , Target.transform.position.y + 1f , 1f), effect =>
                //{
                //    effect.transform.SetParent(GameRoot.Instance.UISystem.WorldCanvas.transform);
                //    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                //    effect.SetAutoRemove(true, 2f);
                //    effect.SetText(10);
                //});
            }
        }
    }



    public void StartFishAction(FishComponent fish)
    {
        fish.FishInBucketAction(this.transform);
    }
}
