using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using BanpoFri;

public class OtterBase : MonoBehaviour
{
    public enum OtterState
    {
        Idle,
        Move,
        Fishing,
        Carry,
    }

    private OtterState CurState = OtterState.Idle;

    public bool IsIdle { get { return CurState == OtterState.Idle; } }

    public bool IsMove { get { return CurState == OtterState.Move; } }

    public bool IsFishing { get { return CurState == OtterState.Fishing; } }

    [SerializeField]
    private float PlayerSpeed = 1f;

    [SerializeField]
    private Transform ProgressTr;

    [SerializeField]
    private Transform FishTr;

    [SerializeField]
    public Transform GetFishTr { get { return FishTr; } }

    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private Transform FishCarryRoot;

    public Transform GetFishCarryRoot { get { return FishCarryRoot; } }

    private List<FishComponent> FishComponentList = new List<FishComponent>();

    public List<FishComponent> GetFishComponentList { get { return FishComponentList; } }

    public bool IsCarry = false;

    private CooltimeProgress Progress;

    public string CurAnimName = "Idle";

    public bool loop = true;

    public void Init()
    {
        GameRoot.Instance.UISystem.LoadFloatingUI<CooltimeProgress>((_progress) => {
            Progress = _progress;
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            Progress.Init(ProgressTr);
            Progress.SetValue(0);
        });


        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;
    }


    public void CoolTimeActive(float cooltimevalue)
    {
        if (Progress == null) return;

        if(cooltimevalue > 0f && !Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, true);
        }

        if(cooltimevalue <= 0f && Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }

        Progress.SetValue(cooltimevalue);
    }


    private void OnDestroy()
    {
        // 콜백 해제
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.End -= HandleEvent;
        }
    }


    private void HandleEvent(Spine.TrackEntry trackEntry)
    {
        Debug.Log($"Animation Complete: {trackEntry.Animation.Name}");


        switch (trackEntry.Animation.Name)
        {
            case "fishingstart":
                {
                    PlayAnimation(OtterState.Fishing,"fishingidle" , true);
                }
                break;
        }
    }

    public void MoveVector(UnityEngine.Vector3 moveVector)
    {
        transform.position += (moveVector * PlayerSpeed);

        PlayAnimation(OtterState.Move,"move", true);

        if (moveVector.x != 0)
        {
            transform.localScale = new UnityEngine.Vector3(moveVector.x > 0 ? -1 : 1, 1, 1);
        }
    }

    public void ChangeState(OtterState state)
    {
        if(state == OtterState.Move && CurState == OtterState.Fishing)
        {
            if(Progress != null && Progress.gameObject.activeSelf)
            {
                ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            }
        }

        if (state == CurState) return;
        
        CurState = state;

    }


    public void CarryStart(bool iscarry)
    {
        IsCarry = iscarry;

    }

    public void AddFish(FishComponent fish)
    {
        FishComponentList.Add(fish);
        CarryStart(FishComponentList.Count > 0);
    }


    public void PlayAnimation(OtterState state, string newAnimationName, bool isLooping)
    {
        if (CurState == state) return;

        var animationname = newAnimationName;

        if(IsCarry)
        {
            switch(animationname)
            {
                case "idle":
                    {
                        animationname = "carryIdle";
                    }
                    break;
                case "move":
                    {
                        animationname = "carry";
                    }
                    break;
                
            }
        }

        ChangeState(state);


        CurAnimName = newAnimationName;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationname, isLooping);
        }
    }
}
