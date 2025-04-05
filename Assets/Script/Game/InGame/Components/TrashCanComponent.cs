using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class TrashCanComponent : MonoBehaviour
{

    private List<OtterBase> OtterList = new List<OtterBase>();
    private Dictionary<FishComponent, float> ProcessingFish = new Dictionary<FishComponent, float>();

    private float TrashTime = 0f;
    private float TrashCanTime = 0.01f;
    private float DestroyTimeout = 1.5f; // 물고기 삭제 타임아웃 시간

    [SerializeField]
    private Transform FishTr;

    [SerializeField]
    private Transform ConsumerTr;

    public Transform GetConsumerTr { get { return ConsumerTr; } }


    public void Init()
    {
        TrashCanTime = 0.01f;
        ProcessingFish.Clear();

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
        // 물고기 처리 상태 체크 및 타임아웃 처리
        CheckProcessingFish();

        if (OtterList.Count == 0) return;

        for (int i = OtterList.Count - 1; i >= 0; i--)
        {
            TrashTime += Time.deltaTime;

            if (TrashCanTime <= TrashTime)
            {
                TrashTime = 0f;

                // 각 Otter의 모든 물고기를 삭제
                var fishList = OtterList[i].GetFishComponentList.ToList(); // 현재 Otter의 모든 물고기 리스트를 복사

                if (fishList.Count == 0) continue;

                // 역순으로 반복
                for (int j = fishList.Count - 1; j >= 0; j--)
                {
                    var fish = fishList[j];
                    
                    if (fish == null) continue; // null 체크 추가
                    
                    // 물고기 처리 목록에 추가
                    if (!ProcessingFish.ContainsKey(fish))
                    {
                        ProcessingFish.Add(fish, Time.time);
                        
                        // 물고기를 처리하고 삭제
                        fish.FishInBucketAction(FishTr, (fishComp) =>
                        {
                            if (fishComp != null) // null 체크
                            {
                                fishComp.transform.SetParent(this.transform);
                                Destroy(fishComp.gameObject);
                                
                                // 처리 완료된 물고기를 목록에서 제거
                                ProcessingFish.Remove(fishComp);
                            }
                        }, 0.2f);
                        
                        // 물고기를 리스트에서 미리 제거
                        OtterList[i].RemoveFish(fish);
                    }
                }

                // 물고기를 모두 제거한 후, Otter가 비어 있으면 CarryEnd() 호출
                if (OtterList[i].GetFishComponentList.Count == 0)
                {
                    OtterList[i].CarryEnd();
                }
            }
        }
    }
    
    // 처리 중인 물고기 상태 체크
    private void CheckProcessingFish()
    {
        // 오래된 물고기 삭제 처리 (타임아웃 시)
        List<FishComponent> toRemove = new List<FishComponent>();
        
        foreach (var pair in ProcessingFish)
        {
            if (Time.time - pair.Value > DestroyTimeout)
            {
                if (pair.Key != null)
                {
                    Debug.LogWarning("물고기 삭제 타임아웃으로 강제 삭제: " + pair.Key.name);
                    Destroy(pair.Key.gameObject);
                }
                toRemove.Add(pair.Key);
            }
        }
        
        // 타임아웃된 물고기 목록에서 제거
        foreach (var fish in toRemove)
        {
            ProcessingFish.Remove(fish);
        }
    }
    
    // 강제로 모든 물고기 삭제 (긴급 상황용)
    public void ForceDestroyAllFish()
    {
        foreach (var otter in OtterList)
        {
            if (otter == null) continue;
            
            var fishList = otter.GetFishComponentList.ToList();
            foreach (var fish in fishList)
            {
                if (fish != null)
                {
                    otter.RemoveFish(fish);
                    Destroy(fish.gameObject);
                }
            }
            
            otter.CarryEnd();
        }
        
        // 처리 중인 물고기도 모두 삭제
        foreach (var pair in ProcessingFish)
        {
            if (pair.Key != null)
            {
                Destroy(pair.Key.gameObject);
            }
        }
        
        ProcessingFish.Clear();
    }
}
