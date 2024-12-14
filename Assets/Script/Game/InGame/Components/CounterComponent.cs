using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CounterComponent : FacilityComponent
{

    public class CounterConsumer
    {
        public int Order = 0;

        public Consumer Consumer;

        public CounterConsumer(int order, Consumer consumer)
        {
            Order = order;
            Consumer = consumer;
        }
    }

    private List<CounterConsumer> CounterConsumerList = new List<CounterConsumer>();

    private float CheckOutConsumerTime = 2f;

    private float checkoutdeltime = 0f;

    private OtterBase Player;

    public override void Init(int facilityidx)
    {
        base.Init(facilityidx);
        CounterConsumerList.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player = collision.gameObject.GetComponent<OtterBase>();

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


    private void Update()
    {
        if(Player != null && CounterConsumerList.Count > 0)
        {
            var findconsumer = CounterConsumerList.Find(x => x.Order == 0 && x.Consumer.IsArrivedCounter);

            if (findconsumer != null)
            {
                checkoutdeltime += Time.deltaTime;

                var valuetime = checkoutdeltime / CheckOutConsumerTime;

                Player.CoolTimeActive(valuetime);

                if (checkoutdeltime >= CheckOutConsumerTime)
                {
                    checkoutdeltime = 0f;

                    GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMoney>(findconsumer.Consumer.transform.position, (effect) =>
                    {
                        effect.SetAutoRemove(true, 1.5f);
                        effect.SetText(100);
                        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, 100);


                        SortConsumer();

                    });

                }
            }
        }
    }

    public void SortConsumer()
    {
        var findconsumer = CounterConsumerList.Find(x => x.Order == 0);

        if (findconsumer != null)
        {
            findconsumer.Consumer.OutCounterConsumer();
            CounterConsumerList.Remove(findconsumer);
        }

        foreach(var consumer in CounterConsumerList)
        {
            if(consumer.Order > 0)
            {
                consumer.Consumer.MovementCounterConsumer(consumer.Order - 1 , () => {
                    consumer.Order -= 1;
                });
            }
        }

    }


    public Transform GetEmptyConsumerTr()
    {
        return ConsumerWaitTr[CounterConsumerList.Count];
    }
    
    public void AddConsumer(Consumer consumer)
    {
        var counterconsumer = new CounterConsumer(CounterConsumerList.Count, consumer);
        CounterConsumerList.Add(counterconsumer);
    }
}
