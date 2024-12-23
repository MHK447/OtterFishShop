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
        var tdlist = Tables.Instance.GetTable<UpgradeInfo>().DataList.ToList();

        foreach(var td in tdlist)
        {
            var getobj = GetCachedObject().GetComponent<UpgradeComponent>();

            if(getobj != null)
            {
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                getobj.Set(td.upgrade_idx);
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
