using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();

    /// <summary>
    /// 总分数
    /// </summary>
    protected int totalScore = 100;

    public bool SyncData = true;

    // 游戏缓存
    public Dictionary<GameMode, ZGameBehaviour> GameTables = new Dictionary<GameMode, ZGameBehaviour>();
    // 当前游戏关卡
    public GameMode CurGameMode = GameMode.Prepare;
    // 当前游戏关卡对应的生命周期
    public ZGameBehaviour CurGameBehaviour = null;

    // 模型角色缓存
    public Dictionary<PlayerRoleModel, GameObject> RoleTables = new Dictionary<PlayerRoleModel, GameObject>();
    // 当前游戏角色
    public PlayerRoleModel CurPlayerRoleModel = PlayerRoleModel.BlackGirl;
    // 当前游戏角色实例
    [HideInInspector] public GameObject CurRole = null;

    // 人物姿态获取api
    public ZPoseHelper PoseHelper;
    private Vector3 PoseHelperDefaultPose;


    public AudioSource BGSound;

    /// <summary>
    /// No.1 action
    /// </summary>
    public GameObject Action1;
    public GameObject Action2;
    public bool A1Hover = false;
    public bool A2Hover = false;

    /// <summary>
    /// tmp 资源过大，不做正常的加载
    /// </summary>
    public GameObject GameScene;

    //public ZBarrierManager BarrierMananger;


    // 角色池缓存
    public Transform RoleDatabase;

    // 是否加入游戏
    public static bool Join = false;

    public bool InitFinish = false;

    private void Awake()
    {
        Instance = this;
        //CurRole = PoseHelper.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (InitFinish)
            CurGameBehaviour.ZUpdate();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeGameMode(GameMode.Drum);
        }
        if (Input.GetKeyDown(KeyCode.G) && Join)
        {
            ChangePlayerRole(CurPlayerRoleModel + 1);
        }
#else
        //if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        //{
        //    ChangeGameMode();
        //}
        //if (NRInput.GetButtonDown(ControllerButton.APP) && Join)
        //{
        //    ChangePlayerRole(CurPlayerRoleModel + 1);
        //}

#endif

        UpdateActionTrigger();

    }

    #region Init

    public void Init()
    {
        if (InitFinish) return;

        CurRole = PoseHelper.transform.GetChild(0).gameObject;


        // 加载人物节点
        PoseHelper.Init(CurPlayerRoleModel);

        // 加载网络和角色表
        InitTables();

        // 加载游戏场景
        LoadGameBehaviour();


        InitFinish = true;
    }


    public void InitBarrierManager()
    {
        //BarrierMananger.InitBarrierConfig();
    }

    public void InitTables()
    {
        S2CFuncTable.Add(S2CFuncName.PoseData, S2C_UpdataPose);
        S2CFuncTable.Add(S2CFuncName.ChangeRole, S2C_UpdatePlayerRole);

        RoleTables.Add(CurPlayerRoleModel, CurRole.gameObject);
    }


    public void LoadGameBehaviour()
    {
        ZGameBehaviour gb;
        if (GameTables.TryGetValue(CurGameMode, out gb))
        {
            CurGameBehaviour = gb;
        }
        else
        {
            switch (CurGameMode)
            {
                case GameMode.Prepare:
                    CurGameBehaviour = new PrepareBehaviour();
                    break;
                //case GameMode.Football:
                //    CurGameBehaviour = new FootballBehaviour();
                //    break;
                case GameMode.Drum:
                    CurGameBehaviour = new DrumBehaviour();
                    break;
                default:
                    break;
            }
            GameTables.Add(CurGameMode, CurGameBehaviour);
        }

        CurGameBehaviour.ZStart();
    }

    Vector3 modelForward; // 模型正方向
    Vector3 cameraForward; // 启动正方向
    bool resetOnce = false;
    public void ResetPositiveDir()
    {
        modelForward = GameObject.FindWithTag("Hip").transform.forward;
        cameraForward = Camera.main.transform.forward;

        float angle = Mathf.Acos(Vector3.Dot(cameraForward, modelForward)) * Mathf.Rad2Deg;

        if (Vector3.Dot(cameraForward - modelForward, Camera.main.transform.right) < 0)
        {
            angle = -angle;
        }

        Vector3 v3 = PoseHelper.transform.rotation.eulerAngles;
        PoseHelper.transform.parent.rotation = Quaternion.Euler(v3.x, v3.y + angle, v3.z);
    }

    #endregion


    #region Game Relevant

    bool guidance = false;
    public void ChangeGameMode(GameMode gm)
    {
        if (CurGameBehaviour != null)
        {
            CurGameBehaviour.ZDisplay(false);
            CurGameBehaviour.ZRelease();
        }
        CurGameMode = (int)gm >= System.Enum.GetNames(typeof(GameMode)).Length ? 0 : gm;

        HideActionTrigger();

        StartCoroutine(PlayReadyAndGuidance(() => 
        {
            // 播放过场&转身动画
            Debug.Log("Play Ready Animation");
            SyncData = false;

        }
        ,() =>
        {
            // 加载完游戏场景开始等到新手引导
            LoadGameBehaviour();
            SyncData = true;


            // 新手引导
            // todo
            Debug.Log("Play Guidance");

        }, 2,
        () =>
        {
            // 开始游戏
            CurGameBehaviour.ZPlay();
            UIManager.Instance.UpdateScore(totalScore);
        }));
    }

    private IEnumerator PlayReadyAndGuidance(Action ReadyPlayEvent = null,Action guidanceEvent = null, float time = 2, Action finishEvent = null)
    {

        yield return null;
        ReadyPlayEvent?.Invoke();

        if (!guidance)
        {
            guidanceEvent?.Invoke();
            yield return new WaitForSeconds(time);
        }

        finishEvent?.Invoke();
    }

    /// <summary>
    /// 切换角色模型
    /// </summary>
    /// <param name="pm"></param>
    public void ChangePlayerRole(PlayerRoleModel pm)
    {
        isWaitingToChangeRole = true;
        pm = (int)pm >= System.Enum.GetNames(typeof(PlayerRoleModel)).Length ? 0 : pm;
        MessageManager.Instance.SendChangeRole((int)pm);

        // 切换人之后，加载开始游戏的button
        AddActionTrigger(CurPlayerRoleModel);

    }

    /// <summary>
    /// 切换背景音乐
    /// </summary>
    /// <param name="clip"></param>
    public void ChangeBgSound(AudioClip clip)
    {
        BGSound.clip = clip;
        BGSound.Play();

        Debug.Log("change to : " + clip.name);
    }


    /// <summary>
    /// 修改分数
    /// </summary>
    /// <param name="v"></param>
    public void SetScore(int v)
    {
        totalScore += v;

        UIManager.Instance.UpdateScore(totalScore);
    }


    public void AddActionTrigger(PlayerRoleModel prm)
    {
        switch (prm)
        {
            case PlayerRoleModel.BlackGirl:

                Action1.SetActive(true);
                Action2.SetActive(false);
                break;
            case PlayerRoleModel.CodeMan:

                Action1.SetActive(false);
                Action2.SetActive(true);

                break;
            default:
                break;
        }
    }

    public void HideActionTrigger()
    {
        Action1.SetActive(false);
        Action2.SetActive(false);
    }

    float actionTriggerTime = 0;
    public void UpdateActionTrigger()
    {
        if (A1Hover && A2Hover)
        {
            actionTriggerTime += Time.fixedDeltaTime;
            if (actionTriggerTime >= ZConstant.HoldTime)
            {
                ChangeGameMode(GameMode.Drum);

                A1Hover = false;
                A2Hover = false;
                actionTriggerTime = 0;
            }
        }
        else
        {
            actionTriggerTime = 0;
        }
    }

    #endregion



    #region S2C

    bool isWaitingToChangeRole = false;
    private void S2C_UpdatePlayerRole(string param)
    {
        PlayerRoleModel role = (PlayerRoleModel)int.Parse(param);

        CurRole = null;

        GameObject model;
        if (RoleTables.TryGetValue(role, out model))
        {
            CurRole = model;
            CurRole.SetActive(true);
        }
        else
        {
            CurRole = GameObject.Instantiate(Resources.Load<GameObject>("Model/" + role.ToString()));
            CurRole.SetActive(true);
            RoleTables[role] = CurRole;
        }

        RoleTables[CurPlayerRoleModel].transform.SetParent(RoleDatabase);
        RoleTables[CurPlayerRoleModel].SetActive(false);

        CurPlayerRoleModel = role;
        CurRole.transform.SetParent(PoseHelper.transform);
        CurRole.transform.localPosition = Vector3.zero;
        CurRole.transform.localScale = Vector3.one;

        PoseHelper.Init(role);

        // 变化角色完成
        isWaitingToChangeRole = false;
    }

    private void S2C_UpdataPose(string param)
    {
        if (isWaitingToChangeRole || !SyncData) return;

        //Debug.Log(param);
        ZPose zp = JsonUtility.FromJson<ZPose>(param);
        //if (zp.role == CurPlayerRoleModel)
        PoseHelper.UpdataPose(zp);

        if (!resetOnce)
        {
            resetOnce = true;
            ResetPositiveDir();
        }

    }

    #endregion



}
