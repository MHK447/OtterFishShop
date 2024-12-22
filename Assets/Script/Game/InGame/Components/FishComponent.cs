using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FishComponent : MonoBehaviour
{
    public enum State
    {
        None,
        Bucket,
        Rack,

    }


    public System.Action<bool> OnEnd = null;

    [SerializeField]
    private SpriteRenderer FishIcon;

    private int FishIdx = 0;

    public int GetFishIdx { get { return FishIdx; } }

    private State CurState;

    private Transform Target;

    private bool IsTracking = false;

    private float TargetYPos = 0f;

    public void Set(int fishidx , State startstate)
    {
        FishIdx = fishidx;

        CurState = startstate;
    }




    public void FishInBucketAction(Transform tr , System.Action<FishComponent> fishaction = null , float time = 1f , float ypos = 0f)
    {
        TargetYPos = ypos;
        IsTracking = false;
        Target = tr;
        // 물고기를 통으로 이동시키는 애니메이션
        this.transform.DOJump(new Vector3(tr.position.x , tr.position.y + ypos , tr.position.z), 3f ,  1  , time).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                IsTracking = true;
                fishaction?.Invoke(this);
            });
    }


    private void Update()
    {
        if(Target != null && IsTracking)
        {
            var targety = Target.position.y + TargetYPos;

            this.transform.position = new Vector3(Target.position.x, targety, Target.position.z);
        }
    }
}
