using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using NavMeshPlus;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Use,
    }

    private PlayerState CurState = PlayerState.Idle;

    [SerializeField]
    private float PlayerSpeed = 1f;

    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    public string CurAnimName = "Idle";       
    public bool loop = true;

    //private void Awake()
    //{
    //    Agent.updateRotation = false;
    //}


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
