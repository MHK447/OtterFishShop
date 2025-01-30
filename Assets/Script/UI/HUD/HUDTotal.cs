using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

[UIPath("UI/Page/HUDTotal", true)]
public class HUDTotal : UIBase
{
    [SerializeField]
    private HudCurrencyTop CurrencyTop;


    [SerializeField]
    private Button UpgradeBtn;

    [SerializeField]
    private Button NextStageBtn;

    [SerializeField]
    private Text FpsText;

    private float deltaTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
        NextStageBtn.onClick.AddListener(OnClickNextStage);
    }

    public void OnClickNextStage()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupNextStage>(popup => popup.Init());
    }


    public void OnClickUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => popup.Init());
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FpsText.text = $"FPS: {Mathf.CeilToInt(fps)}";
    }
}
