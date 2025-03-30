using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class TrashCanComponent : MonoBehaviour
{

    private List<OtterBase> OtterList = new List<OtterBase>();

    private float TrashTime = 0f;

    private float TrashCanTime = 0.01f;

    [SerializeField]
    private Transform FishTr;

    [SerializeField]
    private Transform ConsumerTr;

    public Transform GetConsumerTr { get { return ConsumerTr; } }


    public void Init()
    {
        TrashCanTime = 0.01f;


        GameRoot.Instance.UISystem.LoadFloatingUI<UI_TrashCanBubble>((_progress) =>
        {
            _progress.Init(this.transform);
        });
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher")))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (!OtterList.Contains(getvalue))
                {
                    OtterList.Add(getvalue);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (OtterList.Contains(getvalue))
                {
                    OtterList.Remove(getvalue);
                }
            }
        }
    }


    public void Update()
    {
        if (OtterList.Count == 0) return;

        for (int i = OtterList.Count - 1; i >= 0; i--)
        {
            TrashTime += Time.deltaTime;

            if (TrashCanTime <= TrashTime)
            {
                TrashTime = 0f;

                // 각 Otter의 모든 물고기를 삭제
                var fishList = OtterList[i].GetFishComponentList.ToList(); // 현재 Otter의 모든 물고기 리스트를 복사

                // 역순으로 반복
                for (int j = fishList.Count - 1; j >= 0; j--)
                {
                    var fish = fishList[j];

                    // 물고기를 처리하고 삭제
                    fish.FishInBucketAction(FishTr, (fish) =>
                    {
                        fish.transform.SetParent(this.transform);
                        Destroy(fish.gameObject);  // 물고기 삭제
                    }, 0.2f);

                    // 물고기를 리스트에서 제거
                    OtterList[i].RemoveFish(fish);
                }

                // 물고기를 모두 제거한 후, Otter가 비어 있으면 CarryEnd() 호출
                if (OtterList[i].GetFishComponentList.Count == 0)
                {
                    OtterList[i].CarryEnd();
                }
            }
        }

    }
}
