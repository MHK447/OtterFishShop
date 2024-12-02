using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;


public class Consumer : Chaser
{
    public override void Init(int idx , Transform starttr)
    {
        base.Init(idx , starttr);

        if (starttr != null)
        {
            SetDestination(starttr);
        }

    }
}
