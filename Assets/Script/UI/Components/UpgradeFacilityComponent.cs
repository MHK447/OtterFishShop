using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using TMPro;



public class UpgradeFacilityComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI BefroreValueText;

    [SerializeField]
    private TextMeshProUGUI AfterValueText;

    [SerializeField]
    private TextMeshProUGUI CurCostValueText;

    [SerializeField]
    private TextMeshProUGUI LevelText;
    
    [SerializeField]
    private ButtonPressed UpgradeBtn;
    
    private int FacilityIdx = 0;


    private void Awake()
    {
        UpgradeBtn.OnPressed = () => OnClickUpgrade();
    }
    

    public void Set(int facilityidx)
    {
        FacilityIdx = facilityidx;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, FacilityIdx));

        if(td != null)
        {

        }
    }


    public void SetInfo()
    {

    }

    public void OnClickUpgrade()
    {

    }
}
