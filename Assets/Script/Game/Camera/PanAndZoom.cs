    using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary> A modular and easily customisable Unity MonoBehaviour for handling swipe and pinch motions on mobile. </summary>
public class PanAndZoom : MonoBehaviour
{

    /// <summary> Called as soon as the player touches the screen. The argument is the screen position. </summary>
    public event Action<Vector2> onStartTouch;
    /// <summary> Called as soon as the player stops touching the screen. The argument is the screen position. </summary>
    public event Action<Vector2> onEndTouch;
    /// <summary> Called if the player completed a quick tap motion. The argument is the screen position. </summary>
    //public event Action<Vector2> onTap;

    [Header("Tap")]
    [Tooltip("The maximum movement for a touch motion to be treated as a tap")]
    public float maxDistanceForTap = 40;
    [Tooltip("The maximum duration for a touch motion to be treated as a tap")]
    public float maxDurationForTap = 0.4f;

    [Header("Desktop debug")]
    [Tooltip("Use the mouse on desktop?")]
    public bool useMouse = true;
    [Tooltip("The simulated pinch speed using the scroll wheel")]
    public float mouseScrollSpeed = 2;

    [Header("Camera control")]
    [Tooltip("Does the script control camera movement?")]
    public bool controlCamera = true;
    [Tooltip("The controlled camera, ignored of controlCamera=false")]
    public Camera cam;
    public bool IsZoomOutOver { get { return zoomOutSize < cam.orthographicSize; } }

    [Header("UI")]
    [Tooltip("Are touch motions listened to if they are over UI elements?")]
    public bool ignoreUI = false;

    [Header("Bounds")]
    [Tooltip("Is the camera bound to an area?")]
    public bool useBounds;


    public float boundMinX = -150;
    public float boundMaxX = 150;
    public float boundMinY = -150;
    public float boundMaxY = 150;

    [Header("Etc")]
    public float focusSize = 15f;
    public float focusMoveDuration = 2f;
    public float zoomOutSizeDefault = 20f;

    [SerializeField] float smoothSpeed = 4f;

    bool follow = false;
    Transform followTrans;
    bool focusing = false;
    bool moving = false;
    float dragSpeed = 5f;
    Vector3 movingTarget = Vector3.zero;
    Vector3 distanceOrigin = Vector3.zero;
    Vector3 focusTargetPos;
    Vector3 focusOriginPos;
    float focusOriginCameraSize = 0f;
    float focusDeltaTime = 0f;
    Vector2 touch0StartPosition;
    Vector2 touch0LastPosition;
    float touch0StartTime;
    [HideInInspector]
    public float zoomOutSize = 0f;

    bool cameraControlEnabled = true;

    public Action OnFoucusEnd = null;

    bool canUseMouse;

    private bool IsFocusing = false;

    /// <summary> Has the player at least one finger on the screen? </summary>
    public bool isTouching { get; private set; }

    /// <summary> The point of contact if it exists in Screen space. </summary>
    public Vector2 touchPosition { get { return touch0LastPosition; } }
    private float timeRealDragStop;
    private Vector3 cameraScrollVelocity;
    private AnimationCurve autoScrollDampCurve = new AnimationCurve(new Keyframe(0, 1, 0, 0), new Keyframe(0.7f, 0.9f, -0.5f, -0.5f), new Keyframe(1, 0.01f, -0.85f, -0.85f));
    private Vector3 camVelocity = Vector3.zero;
    private Vector3 posLastFrame = Vector3.zero;
    private bool multiTouch = false;
    Transform Target;
    Transform PlayerTarget;

    void Start()
    {
        var aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        var isTablet = (BanpoFri.Utility.DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);

        if (isTablet)
        {
            boundMinX = -5.0f;
            boundMaxX = 5.0f;
        }
        else
        {
            boundMinX = -4.0f;
            boundMaxX = 4.0f;
        }

        IsFocusing = false;

        canUseMouse = Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer && Input.mousePresent;

        PlayerTarget = Target = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().GetPlayer.transform;
        cam.orthographicSize = 13;
        //zoomOutSize = cam.orthographicSize = Mathf.Min(cam.orthographicSize, (Screen.height * (boundMaxX - boundMinX) / (2 * Screen.width)) - 0.001f);
    }

    Vector3 velocity = Vector3.zero; // 클래스 변수로 선언
    void FixedUpdate()
    {
        if (Target == null) return;
        if (IsFocusing) return;

        Vector3 targetPosition = Target.transform.position;
        targetPosition.z = -10f; // z 값 고정

        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, 0.15f);

    }


    public void FoucsPosition(Transform target)
    {
        IsFocusing = true;
        this.transform.DOMove(new Vector3(target.position.x, target.position.y, -10f), 1f);
    }

    public void FocusOff()
    {
        Target = PlayerTarget;
        IsFocusing = false;
    }

    private float EvaluateAutoScrollDampCurve(float t)
    {
        if (autoScrollDampCurve == null || autoScrollDampCurve.length == 0)
        {
            return (1);
        }
        return autoScrollDampCurve.Evaluate(t);
    }



    /// <summary> Checks if the the current input is over canvas UI </summary>
    public bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touchPosition.x, touchPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public Vector2 RandomPointInBounds()
    {
        var paddingX = (Mathf.Abs(boundMinX) + Mathf.Abs(boundMaxX)) * 0.1f;
        var paddingY = (Mathf.Abs(boundMinY) + Mathf.Abs(boundMaxY)) * 0.2f;

        return new Vector2(
            UnityEngine.Random.Range(boundMinX + paddingX, boundMaxX - paddingX),
            UnityEngine.Random.Range(boundMinY + paddingY, boundMaxY - paddingY)
        );
    }

    //private bool IsClickDust()
    //{
    //	var point = cam.ScreenToWorldPoint(Input.mousePosition);
    //	var ray = new Ray2D(point, Vector2.zero);
    //	RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

    //	if (hit.collider != null)
    //	{
    //		var dust = hit.collider.gameObject.GetComponent<Dust>();
    //		if (dust != null)
    //		{
    //			dust.Pressd();
    //			return true;
    //		}
    //	}
    //	return false;
    //}

    public bool IsClickInGameObject(Vector2 touchPosition)
    {
        var point = cam.ScreenToWorldPoint(touchPosition);
        var ray = new Ray2D(point, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            var cc = hit.collider.gameObject.GetComponent<ClickCallback>();
            if (cc != null)
            {
                cc.Click(hit.collider.gameObject.tag);
            }
        }
        return false;
    }

    public void FocusPosition(Vector3 worldPos, float _focusSize = 15f)
    {
        moving = false;
        focusing = true;
        follow = false;
        focusTargetPos = new Vector3(worldPos.x, worldPos.y, cam.transform.position.z);
        focusOriginPos = cam.transform.position;
        focusDeltaTime = 0f;
        focusSize = _focusSize;
        focusOriginCameraSize = cam.orthographicSize;
    }

    public void FollowCameraPos(Transform worldTrans, float _focusSize = 10f)
    {
        moving = false;
        focusing = false;
        follow = true;
        followTrans = worldTrans;
        focusSize = _focusSize;
        focusDeltaTime = 0f;
        focusOriginCameraSize = cam.orthographicSize;
    }

    public void EndFollow()
    {
        follow = false;
    }

    public void FocusOut()
    {
        moving = false;
        focusing = true;
        follow = false;
        focusTargetPos = cam.transform.position;
        focusOriginPos = cam.transform.position;
        focusDeltaTime = 0f;
        focusSize = zoomOutSize;
        focusOriginCameraSize = cam.orthographicSize;
    }

    /// <summary> Cancels camera movement for the current motion. Resets to use camera at the end of the touch motion.</summary>
    public void CancelCamera()
    {
        cameraControlEnabled = false;
    }

}
