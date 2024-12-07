using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class BucketComponent : MonoBehaviour
{
    private bool IsOnEnter = false;

    public float CurMoneyTime = 0f;

    public float TestTime = 1f;


    private Transform Target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CurMoneyTime = 0f;
            IsOnEnter = true;

            Target = other.transform;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
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

            if(CurMoneyTime >= TestTime)
            {
                CurMoneyTime = 0f;
                GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, 10);


                GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMoney>(new Vector3(Target.transform.position.x , Target.transform.position.y + 1f , 1f), effect =>
                {
                    effect.transform.SetParent(GameRoot.Instance.UISystem.WorldCanvas.transform);
                    ProjectUtility.SetActiveCheck(effect.gameObject, true);
                    effect.SetAutoRemove(true, 2f);
                    effect.SetText(10);
                });
            }
        }
    }
}
