using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[EffectPath("Effect/TextEffectMoney",false , true)]
public class TextEffectMoney : Effect
{
    [SerializeField]
    private Text MoneyText;

    public void SetText(int value)
    {
        MoneyText.text = value.ToString();
    }
}
