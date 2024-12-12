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
        

    }


    public System.Action<bool> OnEnd = null;

    [SerializeField]
    private SpriteRenderer FishIcon;

    private int FishIdx = 0;

    public int GetFishIdx { get { return FishIdx; } }

    private State CurState;

    public void Set(int fishidx , State startstate)
    {
        FishIdx = fishidx;

        CurState = startstate;
    }




    public void FishInBucketAction(Vector3 pos , System.Action<FishComponent> fishaction = null , float time = 1f)
    {
        // 물고기를 통으로 이동시키는 애니메이션
        this.transform.DOJump(pos, 3f ,  1  , time).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                fishaction?.Invoke(this);
            });
    }

}
