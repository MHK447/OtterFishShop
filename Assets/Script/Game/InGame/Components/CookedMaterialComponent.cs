using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
 

public class CookedMaterialComponent : MonoBehaviour
{
    private TextCount_UI MaterialTextCountUI;

    [SerializeField]
    private Transform MaterialCountTr;

    [SerializeField]
    private List<Transform> FishTrList = new List<Transform>();

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    private int FishIdx = 0;

    private int MaxCount = 0;

    public int GetFishIdx { get { return FishIdx; } }

    public int MaterialCount { get { return FishComponentList.Count; } }

    public void Set(int fishidx , int maxcount)
    {
        FishIdx = fishidx;

        MaxCount = maxcount;

        GameRoot.Instance.UISystem.LoadFloatingUI<TextCount_UI>((_progress) => {
            MaterialTextCountUI = _progress;
            ProjectUtility.SetActiveCheck(MaterialTextCountUI.gameObject, FishComponentList.Count > 0);
            MaterialTextCountUI.Init(MaterialCountTr);
            MaterialTextCountUI.SetText(FishComponentList.Count, MaxCount);
        });

    }

    public  bool IsMaxCheck()
    {
        return FishComponentList.Count >= MaxCount;
    }

    public Transform GetCurFishTr()
    {
        return FishTrList[FishComponentList.Count];
    }

    public void AddMaterial(FishComponent fish)
    {
        if (IsMaxCheck()) return;


        fish.FishInBucketAction(FishTrList[FishComponentList.Count], (fish)=> {
            fish.transform.SetParent(this.transform);
            fish.transform.position = FishTrList[FishComponentList.Count].position;
        });

        FishComponentList.Add(fish);

        MaterialTextCountUI.SetText(FishComponentList.Count, MaxCount);

        ProjectUtility.SetActiveCheck(MaterialTextCountUI.gameObject, FishComponentList.Count > 0);

        if (FishComponentList.Count > 0)
            MaterialTextCountUI.Init(FishTrList[FishComponentList.Count - 1]);
    }

        public void RemoveMaterial()
    {
        if (FishComponentList.Count <= 0) return;

        var lastfish = FishComponentList.Last();

        FishComponentList.Remove(lastfish);

        lastfish.ClearObj();

        MaterialTextCountUI.SetText(FishComponentList.Count, MaxCount);

        ProjectUtility.SetActiveCheck(MaterialTextCountUI.gameObject, FishComponentList.Count > 0);

        if (FishComponentList.Count > 0)
            MaterialTextCountUI.Init(FishTrList[FishComponentList.Count - 1]);
    }
}
