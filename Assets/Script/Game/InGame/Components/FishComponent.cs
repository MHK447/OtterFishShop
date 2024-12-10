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

    private State CurState;

    public void Set(int fishidx , State startstate)
    {
        FishIdx = fishidx;

        CurState = startstate;
    }




    public void FishInBucketAction(Transform buckettr)
    {
        // 물고기를 통으로 이동시키는 애니메이션
        this.transform.DOJump(buckettr.position, 3f ,  1  , 1.5f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 애니메이션 완료 후 추가 작업
                Debug.Log("물고기가 통 안에 들어갔습니다!");
            });
    }

}
