using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CounterComponent : FacilityComponent
{

    private List<Consumer> CounterConsumerList = new List<Consumer>();

    private float CheckOutConsumerTime = 2f;

    private float checkoutdeltime = 0f;

    public override void Init()
    {
        base.Init();
        CounterConsumerList.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            checkoutdeltime = 0f;
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Player != null)
            {
                Player.CoolTimeActive(0f);
            }

            checkoutdeltime = 0f;

            Player = null;
        }
    }


    public override void Update()
    {
        base.Update();

        if(Player != null && CounterConsumerList.Count > 0)
        {
            var findconsumer = CounterConsumerList.Find(x => x.CurCounterOrder == 0 && x.IsArrivedCounter);

            if (findconsumer != null)
            {
                checkoutdeltime += Time.deltaTime;

                var valuetime = checkoutdeltime / CheckOutConsumerTime;

                Player.CoolTimeActive(valuetime);

                if (checkoutdeltime >= CheckOutConsumerTime)
                {
                    checkoutdeltime = 0f;

                    GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMoney>(findconsumer.transform.position, (effect) =>
                    {
                        effect.SetAutoRemove(true, 1.5f);
                        effect.SetText(100);
                        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, 100);


                        var findconsumer = CounterConsumerList.Find(x => x.CurCounterOrder == 0);

                        if (findconsumer != null)
                        {
                            findconsumer.OutCounterConsumer();
                            CounterConsumerList.Remove(findconsumer);
                        }
                    });
                }
            }
        }
    }

    public Consumer FindOrderConsumer(int order)
    {
        var finddata = CounterConsumerList.Find(x => x.CurCounterOrder == order);

        if(finddata != null)
        {
            return finddata;
        }

        return null;
    }

    public Transform GetEmptyConsumerTr()
    {
        return ConsumerWaitTr[CounterConsumerList.Count];
    }
    
    public void AddConsumer(Consumer consumer)
    {
        consumer.CurCounterOrder = CounterConsumerList.Count;
        CounterConsumerList.Add(consumer);
    }
}
