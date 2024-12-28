using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;


[UIPath("UI/Popup/PopupUpgrade")]
public class PopupUpgrade : UIBase
{
    [SerializeField]
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private GameObject CachedPrefab;

    [SerializeField]
    private Transform CachedRoot;


    public void Init()
    {

        foreach(var cachedobj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(cachedobj, false);
        }

         

        foreach(var upgradedata in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
        {
            if (upgradedata.IsBuyCheck) continue;

            var getobj = GetCachedObject().GetComponent<UpgradeComponent>();

            if(getobj != null)
            {
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                getobj.Set(upgradedata.UpgradeIdx);
            }
        }
    }


    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(CachedPrefab);
            inst.transform.SetParent(CachedRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
    }
}
