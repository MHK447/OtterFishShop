using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx.Triggers;
using UniRx;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool interactable = true;
    public bool Interactable
    {
        get
        {
            return interactable;
        }
        set
        {
            if (interactable != value)
            {
                if (!value)
                {
                    if (animator != null)
                        animator.Play("Disabled", 0, 0f);
                    pressedCnt = 0;
                }
                else
                {
                    if (animator != null)
                        animator.Play("Normal", 0, 0f);
                }
            }
            interactable = value;
        }
    }

    public bool IsBtnSound = true;


    public float click_interval = 0.5f;
    public float click_interval_fast = 0.03f;
    public int fastPressCnt = 10;
    private bool pressed = false;
    private float deltaTime = 0f;
    private int pressedCnt = 0;
    public System.Action OnPressed = null;
    private Animator animator;
    private IDisposable disposable = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        //yield return new WaitForSeconds(.2f);
        if (animator != null)
        {
            var aniTrigger = animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (aniTrigger != null)
            {
                disposable = aniTrigger.OnStateUpdateAsObservable()
                    .Subscribe(x =>
                    {
                        if (!interactable && x.StateInfo.IsName("Normal"))
                        {
                            animator.Play("Disabled", 0, 0f);
                        }
                    });
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable)
            return;

        pressed = false;
        if (pressedCnt < 1)
        {
            if (animator != null)
                animator.Play("Pressed", 0, 0f);
            OnPressed?.Invoke();

            if (IsBtnSound)
                SoundPlayer.Instance.PlaySound("btn");
        }
        pressedCnt = 0;
        deltaTime = 0f;

    }


    private void OnDisable()
    {
        if (disposable != null)
            disposable.Dispose();
        pressed = false;
        pressedCnt = 0;
        deltaTime = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnCancel()
    {
        pressed = false;
        pressedCnt = 0;
        deltaTime = 0;
    }

    private void Update()
    {
        if (pressed)
        {
            if (!interactable)
            {
                pressed = false;
                return;
            }

            if ((pressedCnt < fastPressCnt ? click_interval : click_interval_fast) < deltaTime)
            {
                if (animator != null)
                    animator.Play("Pressed", 0, 0f);

                if (IsBtnSound)
                    SoundPlayer.Instance.PlaySound("btn");
                OnPressed?.Invoke();
                ++pressedCnt;
                deltaTime = 0f;
            }
            deltaTime += Time.deltaTime;
        }
    }
}
