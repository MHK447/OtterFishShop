using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;
using UniRx;

public class VehicleSystem : MonoBehaviour
{

    public int ad_ride_time = 0;
    public int ride_cash_value = 0;
    public int ad_retimer_time = 0;

    
    public IReactiveProperty<int> AdVehiceTime  = new ReactiveProperty<int>();

    public bool IsAdEquipVehicle = false;

    public void Create()
    {
        ad_ride_time = Tables.Instance.GetTable<Define>().GetData("ad_ride_time").value;
        ride_cash_value = Tables.Instance.GetTable<Define>().GetData("ride_cash_value").value;
        ad_retimer_time = Tables.Instance.GetTable<Define>().GetData("ad_retimer_time").value;

        AdVehiceTime.Value = 0;
    }



    public void OneSecondUpdate()
    {
        if(IsAdEquipVehicle && AdVehiceTime.Value > 0)
        {
            AdVehiceTime.Value -= 1;
        }
    }

    





}
