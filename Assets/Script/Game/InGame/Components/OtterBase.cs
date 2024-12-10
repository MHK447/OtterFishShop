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
        Use,
    }

    private OtterState CurState = OtterState.Idle;

    [SerializeField]
    private float PlayerSpeed = 1f;

    [SerializeField]
    private Transform ProgressTr;

    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

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




    public void MoveVector(UnityEngine.Vector3 moveVector)
    {
        transform.position += (moveVector * PlayerSpeed);

        PlayAnimation("move", true);

        if (moveVector.x != 0)
        {
            transform.localScale = new UnityEngine.Vector3(moveVector.x > 0 ? -1 : 1, 1, 1);
        }
    }



    public void PlayAnimation(string newAnimationName, bool isLooping)
    {
        if (CurAnimName == newAnimationName) return;

        CurAnimName = newAnimationName;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, newAnimationName, isLooping);
        }
    }
}
