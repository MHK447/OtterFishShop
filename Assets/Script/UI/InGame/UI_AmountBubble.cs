using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

[UIPath("UI/InGame/UI_AmountBubble", false)]
public class UI_AmountBubble : InGameFloatingUI
{
    [SerializeField]
    private Image FacilityIconImg;

    [SerializeField]
    private Text AmountCountText;

    [SerializeField]
    private Image SliderValue;

    private int StartCapacity = 0; 

    public void Set(int facilityidx)
    {
        var td = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if(td != null)
        {
            var facilitydata = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(facilityidx);

            int capacitycount = facilitydata == null ? 0 : facilitydata.CapacityCountProperty.Value;

            StartCapacity = td.start_capacity;

            FacilityIconImg.sprite = Config.Instance.GetIngameImg(td.image);
            AmountCountText.text = $"{capacitycount}/{td.start_capacity}";
        }
    }


    public void SetValue(int count)
    {
        AmountCountText.text = $"{count}/{StartCapacity}";
        ProjectUtility.SetActiveCheck(this.gameObject, count > 0);
        SliderValue.fillAmount = (float)count / (float)StartCapacity;
    }

}
