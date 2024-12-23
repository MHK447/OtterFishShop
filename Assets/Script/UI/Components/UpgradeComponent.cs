using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;

public class UpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI DescText;

    [SerializeField]
    private TextMeshProUGUI UpgradeNameText;

    [SerializeField]
    private TextMeshProUGUI CostText;

    private int UpgradeIdx = 0;

    private System.Numerics.BigInteger UpgradeCost;

    public void Set(int upgradeidx)
    {

    }

}
