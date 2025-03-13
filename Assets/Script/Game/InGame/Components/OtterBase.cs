using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using BanpoFri;
using UnityEngine.AI;
using System.Linq;
using UniRx;

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
        SleepMove,
    }

    public enum OtterType
    {
        Player,
        CarryCasher,
    }

    protected OtterState CurState = OtterState.Idle;

    public bool IsIdle { get { return CurState == OtterState.Idle || CurState == OtterState.Wait; } }

    public bool IsMove { get { return CurState == OtterState.Move; } }

    public bool IsFishing { get { return CurState == OtterState.Fishing; } }

    [SerializeField]
    private OtterType CurUnitType;

    protected float PlayerSpeed = 1f;

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

    public int sortingOrderBase = 200; // 기본 정렬 순서
    public int offset = 100;

    private CooltimeProgress Progress;

    public string CurAnimName = "Idle";

    public bool loop = true;

    private TextEffectMax TextEffectMax;

    protected InGameStage CurStage;

    protected System.Action AnimAction;

    private CompositeDisposable disposables = new CompositeDisposable();

    protected int StartCarryCount = 0;

    private Renderer Renderer;

    private float lastYPosition;

    public int CarryCasherWorkFacilityIdx = 0;

    protected int default_player_speed = 0;

    private void Awake()
    {
        Renderer = skeletonAnimation.GetComponent<Renderer>();

        lastYPosition = transform.position.y;

        default_player_speed = Tables.Instance.GetTable<Define>().GetData("default_player_speed").value;

        PlayerSpeed = default_player_speed;
    }

    public virtual void Init()
    {
        CasherMoveSpeed = GameRoot.Instance.InGameSystem.casher_move_speed;

        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        if (Progress == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<CooltimeProgress>((_progress) =>
            {
                Progress = _progress;
                ProjectUtility.SetActiveCheck(Progress.gameObject, false);
                Progress.Init(ProgressTr);
                Progress.SetValue(0);
            });
        }
        else
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }

        DataClear();

        var donebuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);

        disposables.Clear();

        SetCapacity();

        foreach (var donebuy in donebuylist)
        {
            donebuy.IsBuyCheckProperty.Subscribe(x =>
            {
                if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.PlayerSpeedUp)
                {
                    SetPlayerSpeed();
                }
                else if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.PlayerCapacityUp
                || donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.TransportStaffCapacityUp)
                {
                    SetCapacity();
                }
            }).AddTo(disposables);
        }

    if(TextEffectMax == null)
    {
        GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMax>(ProgressTr.transform.position, (effect) =>
        {
            effect.Init(ProgressTr);
            TextEffectMax = effect;

            ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, false);
        });
    }
    else
    {
        TextEffectMax.Init(ProgressTr);
        ProjectUtility.SetActiveCheck(TextEffectMax.gameObject,  true);
    }

        if (CurUnitType == OtterType.Player)
        {
            GameRoot.Instance.UserData.CurMode.PlayerData.VehiclePropertyIdx.Subscribe(x =>
            {
                SetPlayerSpeed();
                PlayAnimation(OtterState.Idle, "idle", true);
            }).AddTo(disposables);
        }
    }


    public virtual void SetPlayerSpeed()
    {

        var vehicleidx = GameRoot.Instance.UserData.CurMode.PlayerData.VehiclePropertyIdx.Value;

        var td = Tables.Instance.GetTable<VehicleInfo>().GetData(vehicleidx);

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.PlayerSpeedUp);

        var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);

        if (td != null)
        {
            PlayerSpeed = default_player_speed + ProjectUtility.PercentCalc(default_player_speed, td.buff_value) + getcalcvalue;
        }
        else
        {
            PlayerSpeed = default_player_speed + getcalcvalue;
        }
    }


    public void SetCapacity()
    {
        if (CurUnitType == OtterType.Player)
        {
            var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.PlayerCapacityUp); ;

            StartCarryCount = GameRoot.Instance.InGameSystem.player_start_carry_count + (int)buffvalue;
        }
        else if (CurUnitType == OtterType.CarryCasher)
        {
            var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.TransportStaffCapacityUp); ;

            StartCarryCount = GameRoot.Instance.InGameSystem.carry_casher_count + (int)buffvalue;
        }
    }


    public void CoolTimeActive(float cooltimevalue)
    {
        if (Progress == null) return;

        Progress.SetValue(cooltimevalue);

        if (cooltimevalue > 0f && !Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, true);

            if (GameRoot.Instance.VehicleSystem.IsAdEquipVehicle)
            {
                Progress.SetOffset(new Vector3(0, 0.5f, 0));
            }
            else
            {
                Progress.SetOffset(Vector3.zero);
            }
        }

        if (cooltimevalue <= 0f && Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }
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

        PlayAnimation(OtterState.Move, "move", true);

        if (moveVector.x != 0)
        {
            transform.localScale = new UnityEngine.Vector3(moveVector.x > 0 ? -1 : 1, 1, 1);
        }
    }

    public void ChangeState(OtterState state)
    {
        if (state == OtterState.Move && CurState == OtterState.Fishing)
        {
            if (Progress != null && Progress.gameObject.activeSelf)
            {
                ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            }
        }

        if (state == CurState) return;

        CurState = state;

    }


    public void DataClear()
    {
        foreach (var fish in FishComponentList)
        {
            fish.ClearObj();
        }

        FishComponentList.Clear();


        if (Progress != null)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }


    }



    public void IdleChange()
    {
        if (Progress != null && Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }

        PlayAnimation(OtterBase.OtterState.Idle, "idle", true);
        ChangeState(OtterState.Idle);

    }

    public virtual void Update()
    {
        // Y축 위치가 변경되었을 때만 정렬 업데이트
        if (Mathf.Abs(transform.position.y - lastYPosition) > Mathf.Epsilon)
        {
            lastYPosition = transform.position.y;
            //UpdateSortingOrder();
        }
    }

    // private void UpdateSortingOrder()
    // {
    //     // Y축 값을 기반으로 Sorting Order 설정
    //     Renderer.sortingOrder = sortingOrderBase - Mathf.RoundToInt(transform.position.y * 1);
    // }

    public void CarryStart(bool iscarry)
    {
        IsCarry = iscarry;

        if (iscarry)
        {
            PlayAnimation(OtterBase.OtterState.Carry, "idle", true);
        }
        else
        {
            ChangeState(OtterBase.OtterState.Wait);
            PlayAnimation(OtterBase.OtterState.Idle, "idle", true);
        }
    }

    public virtual void AddFish(FishComponent fish)
    {
        FishComponentList.Add(fish);
        CarryStart(FishComponentList.Count > 0);
        SoundPlayer.Instance.PlaySound("pickup");


        if (FishComponentList.Count >= StartCarryCount)
        {
            ChangeState(OtterState.Idle);
        }

        TextEffectMaxCheck();

        var fishcount = GetFishComponentList.Count;

        var floory = (0.15f * (fishcount - 1));

        fish.FishInBucketAction(GetFishCarryRoot.transform, (fish) =>
        {
            fish.LivingFishAnim(false);
        }, 0.25f, floory);
    }

    public void CarryEnd()
    {
        CarryStart(false);
    }


    public void RemoveFish(FishComponent fish)
    {
        FishComponentList.Remove(fish);
        SoundPlayer.Instance.PlaySound("putdown");
        TextEffectMaxCheck();
    }

    public void TextEffectMaxCheck()
    {
        if (TextEffectMax != null)
            ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, FishComponentList.Count >= StartCarryCount);
    }

    public bool IsMaxFishCheck()
    {
        return FishComponentList.Count >= StartCarryCount;
    }

    public FishComponent GetFacilityFish(int fishidx)
    {
        var findfish = FishComponentList.Find(x => x.GetFishIdx == fishidx);

        if (findfish != null)
        {
            return findfish;
        }

        return null;
    }


    public void SortFish()
    {
        for (int i = 0; i < FishComponentList.Count; ++i)
        {
            var floory = (0.15f * i);
            FishComponentList[i].FishInBucketAction(GetFishCarryRoot.transform, (fish) =>
            {
            }, 0f, floory);
        }
    }



    public void PlayAnimation(OtterState state, string newAnimationName, bool isLooping)
    {
        if (CurState == state) return;

        string aniname = GetVehicleAnim(state);

        ChangeState(state);

        CurAnimName = aniname;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, aniname, isLooping);
        }
    }




    ///casher///

    public Transform TargetTr;

    protected bool _isMoving = false;

    Coroutine _currentMoveProcess;

    [SerializeField] private float reachRadius = 0.60f;

    [SerializeField] protected NavMeshAgent _navMeshAgent;

    protected float CasherMoveSpeed = 4f;

    WaitForSeconds _waitTick;

    float _agentDrift = 0.0001f;

    Vector3 _destinationPosition;


    [SerializeField]
    private CasherType CasherType;

    [HideInInspector]
    public int GetCasherIdx { get { return (int)CasherType; } }

    public void SetDestination(Transform destination, System.Action arrivedAction)
    {
        TargetTr = destination;
        _isMoving = true;

        // 목표 지점과의 거리 계산 (더 정밀하게)
        if (Vector2.Distance(transform.position, destination.position) < Mathf.Epsilon)
        {
            ReachProcess();
            arrivedAction?.Invoke();
            return;
        }

        // 길찾기 경로 설정 (WayPoints 조정)
        SetWayPoints();

        // 이동 코루틴 실행 (중복 실행 방지)
        if (_currentMoveProcess == null)
        {
            _currentMoveProcess = StartCoroutine(MoveProcess(arrivedAction));
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


    private IEnumerator MoveProcess(System.Action arrivedAction = null)
    {
        while (_wayPoints.Count > 0)
        {
            var currentWayPoint = _wayPoints[0];

            // 목표 지점과의 거리 계산
            float distance = Vector2.Distance(transform.position, currentWayPoint);

            if (distance <= reachRadius)
            {
                _wayPoints.RemoveAt(0);

                // 최종 도착 처리 (부드럽게)
                if (_wayPoints.Count == 0)
                {
                    StartCoroutine(SmoothArrival(TargetTr.position, arrivedAction));
                    yield break;
                }
            }
            else
            {
                _isMoving = true;
                var animname = IsCarry ? "carry" : "move";

                PlayAnimation(OtterState.Move, animname, true);

                transform.localScale = new Vector3(transform.position.x - currentWayPoint.x > 0 ? 1f : -1f, 1f, 1f);

                // 🔹 이동 속도 제한 (현재 속도보다 더 멀리 이동하지 않도록 보정)
                float step = Mathf.Min(Time.deltaTime * CasherMoveSpeed, distance);
                transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, step);
            }

            yield return _waitTick;
        }

        _currentMoveProcess = null;
        ReachProcess();
        arrivedAction?.Invoke();
    }



    // 🔹 최종 도착을 부드럽게 처리하는 함수
    IEnumerator SmoothArrival(Vector2 finalPosition, System.Action arrivedAction)
    {
        float duration = 0.2f; // 최종 도착에 걸리는 시간 (부드럽게 감속)
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * (2 - t); // Ease-out (부드럽게 감속)

            transform.position = Vector2.Lerp(startPos, finalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition; // 정확한 위치 보정
        _currentMoveProcess = null;
        ReachProcess();
        arrivedAction?.Invoke();
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

    private void OnDisable()
    {
        if (Progress != null)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            Destroy(Progress.gameObject);
            Progress = null;
        }

        if (TextEffectMax != null)
        {
            ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, false);
            Destroy(TextEffectMax.gameObject);
            TextEffectMax = null;
        }
    }

    public string GetVehicleAnim(OtterState state)
    {
        var vehicleidx = GameRoot.Instance.UserData.CurMode.PlayerData.VehiclePropertyIdx.Value;

        var td = Tables.Instance.GetTable<VehicleInfo>().GetData(vehicleidx);

        if (td != null && CurUnitType == OtterType.Player)
        {
            switch (state)
            {
                case OtterState.Wait:
                case OtterState.Idle:
                    return IsCarry ? $"vehicle_{vehicleidx}_carryidle" : $"vehicle_{vehicleidx}_idle";
                case OtterState.Fishing:
                    return $"vehicle_{vehicleidx}_fishingidle";
                case OtterState.Move:
                    return IsCarry ? $"vehicle_{vehicleidx}_carry" : $"vehicle_{vehicleidx}_move";
                case OtterState.Carry:
                    return IsCarry ? $"vehicle_{vehicleidx}_carryidle" : $"vehicle_{vehicleidx}_idle";
            }
        }
        else
        {
            switch (state)
            {
                case OtterState.Wait:
                case OtterState.Idle:
                    return IsCarry ? "carryidle" : "idle";
                case OtterState.Fishing:
                    return "fishingidle";
                case OtterState.Move:
                    return IsCarry ? "carry" : "move";
                case OtterState.Sleep:
                    return "napstart";
                case OtterState.Carry:
                    return IsCarry ? "carryidle" : "idle";

            }
        }

        return string.Empty;
    }

    private void OnDestroy()
    {
        if (Progress != null)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
            Destroy(Progress.gameObject);
            Progress = null;
        }


        if (TextEffectMax != null)
        {
            ProjectUtility.SetActiveCheck(TextEffectMax.gameObject, false);
            Destroy(TextEffectMax);
            TextEffectMax = null;
        }
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
