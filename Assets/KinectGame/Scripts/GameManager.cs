using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();

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
            ChangeGameMode();
        }
        if (Input.GetKeyDown(KeyCode.G) && Join)
        {
            ChangePlayerRole(CurPlayerRoleModel + 1);
        }
#else
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            ChangeGameMode();
        }
        if (NRInput.GetButtonDown(ControllerButton.APP) && Join)
        {
            ChangePlayerRole(CurPlayerRoleModel + 1);
        }

#endif

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
                case GameMode.Football:
                    CurGameBehaviour = new FootballBehaviour();
                    break;
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
        Debug.Log(angle + "----------123-  ");

        if (Vector3.Dot(cameraForward - modelForward, Camera.main.transform.right) < 0)
        {
            angle = -angle;
        }

        Vector3 v3 = PoseHelper.transform.rotation.eulerAngles;
        PoseHelper.transform.parent.rotation = Quaternion.Euler(v3.x, v3.y + angle, v3.z);
    }

    #endregion


    public void ChangeGameMode()
    {
        if (CurGameBehaviour != null)
        {
            CurGameBehaviour.ZDisplay(false);
            CurGameBehaviour.ZRelease();
        }
        CurGameMode = (int)CurGameMode + 1 >= System.Enum.GetNames(typeof(GameMode)).Length ? 0 : CurGameMode + 1;

        LoadGameBehaviour();
    }

    public void ChangePlayerRole(PlayerRoleModel pm)
    {
        isWaitingToChangeRole = true;
        pm = (int)pm >= System.Enum.GetNames(typeof(PlayerRoleModel)).Length ? 0 : pm;
        MessageManager.Instance.SendChangeRole((int)pm);
    }

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
        if (isWaitingToChangeRole) return;

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
