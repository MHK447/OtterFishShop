using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityComponent : MonoBehaviour
{
    [SerializeField]
    protected List<Transform> ConsumerWaitTr = new List<Transform>();

    public int ConsumerOrder = 0;

    public int FacilityIdx = 0;

    public virtual void Init(int facilityidx)
    {
        FacilityIdx = facilityidx;
        ConsumerOrder = 0;
    }

    public virtual Transform GetConsumerTr()
    {
        ConsumerOrder += 1;
        return ConsumerWaitTr[ConsumerOrder - 1];

    }

    

    public Transform GetConsumerTr(int order)
    {
        return ConsumerWaitTr[order];
    }
     
}
