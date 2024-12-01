using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void MoveVector(UnityEngine.Vector3 moveVector)
    {

        transform.position += (moveVector * PlayerSpeed);
        //.localPosition = new UnityEngine.Vector3(0, 0, transform.position.y);

        if (moveVector.x != 0)
        {
            transform.localScale = new UnityEngine.Vector3(moveVector.x > 0 ? -1 : 1, 1, 1);
            //maxStackObject.transform.localScale = new UnityEngine.Vector3(moveVector.x > 0 ? -1 : 1, 1, 1);
        }

        //StartMove();

        //SetBubblePos(transform);
    }
}
