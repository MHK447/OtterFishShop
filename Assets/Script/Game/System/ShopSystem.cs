using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShopSystem
{

    public ReactiveProperty<bool> IsVipProperty = new ReactiveProperty<bool>(false);

    public void Create()
    {

    }
}
