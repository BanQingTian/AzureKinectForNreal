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

    [HideInInspector] public Transform head;

    public AudioSource BGSound;

    /// <summary>
    /// 障碍物的音频
    /// </summary>
    public GameObject BarrierAS;

    /// <summary>
    /// blackgirl action trigger
    /// </summary>
    public GameObject Action1;
    /// <summary>
    /// aottman action trigger
    /// </summary>
    public GameObject Action2;
    // 触碰点A1
    public bool A1Hover = false;
    // 触碰点A2
    public bool A2Hover = false;

    /// <summary>
    /// tmp 资源过大，不做正常的加载,直接甩到场景中
    /// </summary>
    public GameObject GameScene;
    public Animator GameAnim;

    /// <summary>
    /// tmp 资源过大，不做动态加载
    /// </summary>
    public GameObject ChooseMenu;
    public GameObject AottmanRole;

    // 两个角色对应的滑板 Prepare
    public GameObject Stake_Blackgirl_prepare;
    public GameObject Stake_Aottman_prepare;

    // 两个角色对应的滑板 Gaming
    public GameObject Stake_Blackgirl_gaming;
    public GameObject Stake_Aottman_gaming;

    // 两个角色对应的转身特效
    public GameObject Blackgirl_vfx;
    public GameObject Aottman_vfx;

    // 对应角色的边框父节点
    public Transform WallParent;
    // blackgirl角色的边框
    public GameObject BlackgirlWall;
    // blackgiRL角色的边框特效
    public GameObject BlackgirlWallEff;
    //
    public GameObject AottmanWall;
    //
    public GameObject AottmanWallEff;

    [HideInInspector] public Vector3 Groud;
    [HideInInspector] public Vector3 HeadStartPos;

    //public ZBarrierManager BarrierMananger;


    // 角色池缓存
    public Transform RoleDatabase;

    // 是否加入游戏
    public static bool Join = false;

    // 初始化完成
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

        if (CurGameMode == GameMode.Prepare)
        {

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeGameMode(CurGameMode + 1);
            }
            if (Input.GetKeyDown(KeyCode.G) && Join)
            {
                ChangePlayerRole(CurPlayerRoleModel + 1);
            }
#else
            //if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            //{
            //    ChangeGameMode(GameMode.Drum);
            //}
            //if (NRInput.GetButtonDown(ControllerButton.APP) && Join)
            //{
            //    ChangePlayerRole(CurPlayerRoleModel + 1);
            //}
#endif

        }

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
        // 网络消息注册
        S2CFuncTable.Add(S2CFuncName.PoseData, S2C_UpdataPose);
        S2CFuncTable.Add(S2CFuncName.ChangeRole, S2C_UpdatePlayerRole);

        // 角色表注册
        RoleTables.Add(CurPlayerRoleModel, CurRole.gameObject);
    }

    /// <summary>
    /// 主要玩法processing 
    /// </summary>
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


    /// <summary>
    /// 方向初始化，保证在眼镜中的模型初始化朝向是正方向（起因 : kinect可能从斜前方检测身体
    /// </summary>
    Vector3 modelForward; // 模型正方向
    Vector3 cameraForward; // 启动正方向
    bool resetOnce = false;
    float angAdd = 0;
    public void ResetPositiveDir()
    {
        modelForward = GameObject.FindWithTag("Hip").transform.forward;
        cameraForward = Camera.main.transform.forward;

        float angle = Mathf.Acos(Vector3.Dot(cameraForward, modelForward)) * Mathf.Rad2Deg;


        Debug.Log(angle);
        if (Vector3.Dot(cameraForward - modelForward, Camera.main.transform.right) < 0)
        {
            angAdd = angle - 180;
            angle = -angle;
        }
        else
        {
            angAdd = 180 - angle;
        }

        Debug.Log(angAdd);

        Vector3 v3 = PoseHelper.transform.rotation.eulerAngles;
        PoseHelper.transform.parent.rotation = Quaternion.Euler(v3.x, v3.y + angle, v3.z);

        BGSound.Play();


        head = Camera.main.transform;
        Debug.LogError("Camera.main.transform ======= " + Camera.main.transform);
        Groud = PoseHelper.transform.position;
        HeadStartPos = head.position;


        ResetFaceToFace(true);

    }

    /// <summary>
    /// 玩家角色朝向，face=ture 面朝camera， false背对camera
    /// </summary>
    bool once = false;
    public void ResetFaceToFace(bool face = true)
    {
        float rotaY;
        float scaleX;
        if (face)
        {
            rotaY = PoseHelper.transform.rotation.eulerAngles.y + 180;
            scaleX = 1;
        }
        else
        {
            rotaY = -180;
            scaleX = -1;

            rotaY = angAdd;
        }



        Vector3 v3 = PoseHelper.transform.rotation.eulerAngles;
        PoseHelper.transform.parent.rotation = Quaternion.Euler(v3.x, rotaY, v3.z);
        v3 = PoseHelper.transform.parent.localScale;
        PoseHelper.transform.parent.localScale = new Vector3(v3.x * scaleX, v3.y, v3.z);
    }

    #endregion


    public void ResetGame()
    {
        wallMoveaSpeed = 1;
        wallCreateTime = 1;
        A1Hover = false;
        A2Hover = false;
        resetOnce = false;
        updateChangeOnce = false;
    }

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
            if (CurPlayerRoleModel == PlayerRoleModel.Aottman)
            {
                Aottman_vfx.SetActive(true);
                Blackgirl_vfx.SetActive(false);
            }
            else
            {
                Aottman_vfx.SetActive(false);
                Blackgirl_vfx.SetActive(true);
            }

        }, 1.5f,
        () =>
        {
            ResetFaceToFace(false);
            // 加载完游戏场景开始等到新手引导
            LoadGameBehaviour();
            StopWall();
            CreateWall();
            SyncData = true;


            // 新手引导
            // todo
            Debug.Log("Play Guidance");

        }, 2,
        () =>
        {
            UIManager.Instance.StartCountdown(150, () =>
            {
                // 游戏结束
                Debug.Log("gameover");
            });
            Aottman_vfx.SetActive(false);
            Blackgirl_vfx.SetActive(false);

            // 开始游戏
            CurGameBehaviour.ZPlay();
            UIManager.Instance.UpdateScore(totalScore);
        }));
    }

    private IEnumerator PlayReadyAndGuidance(Action ReadyPlayEvent = null, float waitTime = 1.5f, Action guidanceEvent = null, float time = 2, Action finishEvent = null)
    {
        yield return null;
        ReadyPlayEvent?.Invoke();

        yield return new WaitForSeconds(waitTime);

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
        A1Hover = false;
        A2Hover = false;
        isWaitingToChangeRole = true;
        pm = (int)pm >= System.Enum.GetNames(typeof(PlayerRoleModel)).Length ? 0 : pm;
        Debug.Log("change role ~~~~~~~~~~  " + pm);
        MessageManager.Instance.SendChangeRole((int)pm);
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
            case PlayerRoleModel.Aottman:

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
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER) || NRInput.GetButtonDown(ControllerButton.TOUCHPAD_BUTTON))
        {
            ChangeGameMode(GameMode.Drum);
            A1Hover = false;
            A2Hover = false;
            actionTriggerTime = 0;
        }

        if (A1Hover && A2Hover)
        {
            //actionTriggerTime += Time.fixedDeltaTime;
            //if (actionTriggerTime >= ZConstant.HoldTime)
            //{
                ChangeGameMode(GameMode.Drum);

                A1Hover = false;
                A2Hover = false;
                actionTriggerTime = 0;
            //}
        }
        else
        {
            actionTriggerTime = 0;
        }
    }

    public static float wallCreateTime = 1;
    public static float wallMoveaSpeed = 1;
    public void CreateWall()
    {
        StartCoroutine("create");
    }
    public void StopWall()
    {
        StopCoroutine("create");
    }
    private IEnumerator create()
    {
        GameObject wall;
        if (CurPlayerRoleModel == PlayerRoleModel.Aottman)
        {
            AottmanWallEff.SetActive(true);
            BlackgirlWallEff.SetActive(false);
            wall = AottmanWall;
        }
        else
        {
            AottmanWallEff.SetActive(false);
            BlackgirlWallEff.SetActive(true);
            wall = BlackgirlWall;
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (wallCreateTime == 0)
            {
                yield return null;
            }
            else
            {
                GameObject w = PoolManager.Instance.Get(wall);
                var movewall = w.GetComponent<MoveWall>();
                if (movewall == null)
                {
                    movewall = w.AddComponent<MoveWall>();
                }
                movewall.Init(WallParent, new Vector3(0, 0, 40));

                yield return new WaitForSeconds(wallCreateTime);
            }
        }
    }

    public void PlayBarrierAudio(SoundEffEnum se, Vector3 pos)
    {
        GameObject go = PoolManager.Instance.Get(BarrierAS);
        go.SetActive(true);
        go.transform.position = pos;
        AudioSource au = go.GetComponent<AudioSource>();
        au.clip = GameResConfig.Instance.GetAudioEff(se);
        au.Play();
        StartCoroutine(recycle(go));
    }
    private IEnumerator recycle(GameObject audio)
    {
        yield return new WaitForSeconds(1);
        PoolManager.Instance.Release(audio);
    }

    #endregion


    #region S2C

    bool isWaitingToChangeRole = false;
    /// <summary>
    /// 接受server转发的消息，更新玩家角色
    /// </summary>
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
            if (role == PlayerRoleModel.Aottman)
            {
                CurRole = AottmanRole;
                CurRole.SetActive(true);
            }
            //CurRole = GameObject.Instantiate(Resources.Load<GameObject>("Model/" + role.ToString()));
            // CurRole.SetActive(true);
            RoleTables[role] = CurRole;
        }


        if (role == CurPlayerRoleModel)
        {
            isWaitingToChangeRole = false;

            // 切换人之后，加载开始游戏的button
            AddActionTrigger(CurPlayerRoleModel);

            switch (CurPlayerRoleModel)
            {
                case PlayerRoleModel.BlackGirl:
                    Stake_Aottman_prepare.SetActive(false);
                    Stake_Blackgirl_prepare.SetActive(true);
                    break;
                case PlayerRoleModel.Aottman:
                    Stake_Aottman_prepare.SetActive(true);
                    Stake_Blackgirl_prepare.SetActive(false);
                    break;
                default:
                    break;
            }

            return;
        }


        // 更新模型数据
        RoleTables[CurPlayerRoleModel].transform.SetParent(RoleDatabase);
        RoleTables[CurPlayerRoleModel].SetActive(false);

        CurPlayerRoleModel = role;
        CurRole.transform.SetParent(PoseHelper.transform);
        CurRole.transform.localPosition = Vector3.zero;
        CurRole.transform.localScale = Vector3.one;

        PoseHelper.Init(role);

        // 变化角色完成
        isWaitingToChangeRole = false;

        // 切换人之后，加载开始游戏的button
        AddActionTrigger(CurPlayerRoleModel);


        // new feature, change stake
        switch (CurPlayerRoleModel)
        {
            case PlayerRoleModel.BlackGirl:
                Stake_Aottman_prepare.SetActive(false);
                Stake_Blackgirl_prepare.SetActive(true);
                break;
            case PlayerRoleModel.Aottman:
                Stake_Aottman_prepare.SetActive(true);
                Stake_Blackgirl_prepare.SetActive(false);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 接受server转发的消息，更新位置信息
    /// </summary>
    bool updateChangeOnce = true;
    private void S2C_UpdataPose(string param)
    {
        if (isWaitingToChangeRole || !SyncData) return;

        //Debug.Log(param);
        ZPose zp = JsonUtility.FromJson<ZPose>(param);

        if (zp.role == CurPlayerRoleModel)
        {
            PoseHelper.UpdataPose(zp);
        }
        else
        {
            ChangePlayerRole(CurPlayerRoleModel);
        }

        if (!resetOnce)
        {
            resetOnce = true;
            ResetPositiveDir();
        }

    }

    #endregion



}
