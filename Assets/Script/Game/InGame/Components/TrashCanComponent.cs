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

    private float TrashCanTime = 0.2f;

    [SerializeField]
    private Transform FishTr;


    public void Init()
    {
        TrashCanTime = 0.2f;
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
                if(OtterList.Contains(getvalue))
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

            if(TrashCanTime <= TrashTime)
            {
                TrashTime = 0f;

                if(OtterList[i].GetFishComponentList.Count > 0)
                {
                    var findfish = OtterList[i].GetFishComponentList.Last();

                    findfish.FishInBucketAction(FishTr, (fish) => {
                        fish.transform.SetParent(this.transform);
                        Destroy(fish.gameObject);
                    }, 0.2f);

                    OtterList[i].RemoveFish(findfish);
                }
            }
        }
    }
}
