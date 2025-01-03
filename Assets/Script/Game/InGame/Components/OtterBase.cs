using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using BanpoFri;
using UnityEngine.AI;

public class OtterBase : MonoBehaviour
{
    public enum OtterState
    {
        Idle,
        Move,
        Fishing,
        Carry,
        Wait,
        Sleep,
        Work,
    }

    protected OtterState CurState = OtterState.Idle;

    public bool IsIdle { get { return CurState == OtterState.Idle || CurState == OtterState.Wait; } }

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
    protected SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private Transform FishCarryRoot;

    public Transform GetFishCarryRoot { get { return FishCarryRoot; } }

    protected List<FishComponent> FishComponentList = new List<FishComponent>();

    public List<FishComponent> GetFishComponentList { get { return FishComponentList; } }

    public bool IsCarry = false;

    private CooltimeProgress Progress;

    public string CurAnimName = "Idle";

    public bool loop = true;

    private TextEffectMax TextEffectMax;

    protected InGameStage CurStage;

    protected System.Action AnimAction;

    public virtual void Init()
    {
        
        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        GameRoot.Instance.UISystem.LoadFloatingUI<CooltimeProgress>((_progress) => {
            Progress = _progress;
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            Progress.Init(ProgressTr);
            Progress.SetValue(0);
        });


        GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMax>(ProgressTr.transform.position, (effect) =>
        {
            effect.Init(ProgressTr);
            TextEffectMax = effect;

            ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, false);
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

        if(Progress.gameObject.activeSelf)
        Progress.SetValue(cooltimevalue);
    }


    public void CoolTimeActive(bool isactive)
    {
        if (Progress == null) return;

        if (!Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, isactive);
        }

        if (Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, isactive);
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


        if (FishComponentList.Count >= 5)
        {
            ChangeState(OtterState.Idle);
        }

        TextEffectMaxCheck();

    }


    public void RemoveFish(FishComponent fish)
    {
        FishComponentList.Remove(fish);
        TextEffectMaxCheck();
    }

    public void TextEffectMaxCheck()
    {
        if(TextEffectMax != null)
        ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, FishComponentList.Count >= 5);
    }

    public bool IsMaxFishCheck()
    {
        return FishComponentList.Count >= 5;
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




    ///casher///

    public Transform TargetTr;

    protected bool _isMoving = false;

    Coroutine _currentMoveProcess;

    [SerializeField] private float reachRadius = 0.60f;

    [SerializeField] protected NavMeshAgent _navMeshAgent;

    protected float CurrentMoveSpeed = 4f;

    WaitForSeconds _waitTick;

    float _agentDrift = 0.0001f;

    Vector3 _destinationPosition;


    [SerializeField]
    private CasherType CasherType;

    [HideInInspector]
    public int GetCasherIdx {  get { return (int)CasherType; } }

    public void SetDestination(Transform destination, System.Action arrivedaction)
    {
        TargetTr = destination;

        _isMoving = true;

        if (((Vector2)transform.position - (Vector2)destination.position).magnitude < 0.1f)
        {
            ReachProcess();
        }
        else
        {
            var driftPos = destination.position;
            if (Mathf.Abs(transform.position.x - destination.position.x) < _agentDrift)
            {
                driftPos = destination.position + new Vector3(_agentDrift, 0f, 0f);
            }

            _destinationPosition = driftPos;
            SetWayPoints();

            if (_currentMoveProcess == null)
            {
                _currentMoveProcess = StartCoroutine(MoveProcess(arrivedaction));
            }
        }
    }

    List<Vector2> _wayPoints = new List<Vector2>();

    public void SetWayPoints()
    {
        if (TargetTr == null) return;

        Vector2 targetPosition = TargetTr.transform.position;
        _wayPoints.Clear();

        NavMeshPath navMeshPath = new NavMeshPath();
        if (_navMeshAgent.CalculatePath(targetPosition, navMeshPath))
        {
            // 우회 경로 생성
            Vector2[] wayPoints = SmoothPath(navMeshPath.corners);

            for (int i = 0; i < wayPoints.Length; i++)
            {
                _wayPoints.Add(wayPoints[i]);
            }
        }
        else
        {
            Debug.Log("(!) Fail to NavMeshAgent's CalculatePath : " + this.gameObject.name);
            this.transform.position = CurStage.GetStartWayPoint.position;
        }

        _wayPoints.Add(targetPosition);
    }


    public void ReachProcess()
    {
        _isMoving = false;
        PlayAnimation(OtterState.Idle, "idle", true);
    }


    IEnumerator MoveProcess(System.Action arrivedaction = null)
    {
        while (_wayPoints.Count > 0)
        {
            var currentWayPoint = _wayPoints[0];
            if (((Vector2)transform.position - currentWayPoint).magnitude < reachRadius)
            {
                _wayPoints.RemoveAt(0);
            }
            else
            {
                _isMoving = true;

                var animname = IsCarry ? "carry" : "move";

                PlayAnimation(OtterState.Move, animname, true);

                transform.localScale = new Vector3(transform.position.x - currentWayPoint.x > 0 ? 1f : -1f, 1f, 1f);
                transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, Time.deltaTime * CurrentMoveSpeed);
            }

            yield return _waitTick;
        }

        _currentMoveProcess = null;
        arrivedaction?.Invoke();
        ReachProcess();

        yield break;
    }


    Vector2[] SmoothPath(Vector3[] pathCorners)
    {
        List<Vector2> smoothPath = new List<Vector2>();

        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Vector2 start = pathCorners[i];
            Vector2 end = pathCorners[i + 1];

            // 현재 세그먼트의 시작 지점을 추가
            smoothPath.Add(start);

            // 현재 세그먼트의 길이를 계산
            float segmentLength = Vector2.Distance(start, end);

            // 현재 세그먼트를 충분히 세분화하여 추가
            int divisions = Mathf.CeilToInt(segmentLength / reachRadius);
            for (int j = 1; j <= divisions; j++)
            {
                float t = j / (float)divisions;
                smoothPath.Add(Vector2.Lerp(start, end, t));
            }
        }

        return smoothPath.ToArray();
    }




    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();


        if (this.gameObject.activeSelf)
        {
            SetWayPoints();
        }
    }



}
