using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

public class FacilityComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject FacilityObj;

    [SerializeField]
    private GameObject FacilityContentsObj;

    [SerializeField]
    private Image FacilityImg;

    [SerializeField]
    protected List<Transform> ConsumerWaitTr = new List<Transform>();

    [SerializeField]
    private Transform NewRoot;

    public int ConsumerOrder = 0;

    public int FacilityIdx = 0;

    private bool IsOpen = false;

    private NewFacilityUI NewFacilityUI;

    public virtual void Init(int facilityidx)
    {
        FacilityIdx = facilityidx;
        ConsumerOrder = 0;

        GameRoot.Instance.UISystem.LoadFloatingUI<NewFacilityUI>((_newfacility) => {
            NewFacilityUI = _newfacility;
            ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, false);
            _newfacility.Init(NewRoot);
        });




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
