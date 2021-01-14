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
    public GameMode CurGameMode = GameMode.Football;
    // 当前游戏关卡对应的生命周期
    public ZGameBehaviour CurGameBehaviour = null;

    // 模型角色缓存
    public Dictionary<PlayerRoleModel, GameObject> RoleTables = new Dictionary<PlayerRoleModel, GameObject>();
    // 当前游戏角色
    public PlayerRoleModel CurPlayerRoleModel = PlayerRoleModel.BlackGirl;
    // 当前游戏角色实例
    public GameObject CurRole = null;

    // 人物姿态获取api
    public ZPoseHelper PoseHelper;

    public GameObject RoleBase;

    // 是否加入游戏
    public static bool Join = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CurRole = PoseHelper.transform.GetChild(0).gameObject;

        LoadGame();
    }

    private void Update()
    {
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

    public void LoadGame()
    {
        // 加载人物节点
        PoseHelper.Init(CurPlayerRoleModel);

        // 加载网络和角色表
        InitTables();

        // 加载游戏场景
        InitGameBehaviour();
    }


    #region Init

    public void InitTables()
    {
        S2CFuncTable.Add(S2CFuncName.PoseData, S2C_UpdataPose);
        S2CFuncTable.Add(S2CFuncName.ChangeRole, S2C_UpdatePlayerRole);

        RoleTables.Add(CurPlayerRoleModel, CurRole);
    }


    public void InitGameBehaviour()
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

    #endregion


    public void ChangeGameMode()
    {
        if (CurGameBehaviour != null)
        {
            CurGameBehaviour.ZDisplay(false);
            // CurGameBehaviour.ZRelease();
        }
        CurGameMode = (int)CurGameMode + 1 >= System.Enum.GetNames(typeof(GameMode)).Length ? 0 : CurGameMode + 1;

        InitGameBehaviour();
    }

    public void ChangePlayerRole(PlayerRoleModel pm)
    {
        pm = (int)pm >= System.Enum.GetNames(typeof(PlayerRoleModel)).Length ? 0 : pm;
        MessageManager.Instance.SendChangeRole((int)pm);
    }

    #region S2C

    private void S2C_UpdatePlayerRole(string param)
    {
        PlayerRoleModel role = (PlayerRoleModel)int.Parse(param);

        if (CurRole != null)
        {
            GameObject.Destroy(CurRole.gameObject);
        }

        GameObject model;
        if (RoleTables.TryGetValue(role, out model))
        {
            CurRole = GameObject.Instantiate(Resources.Load<GameObject>("Model/" + role.ToString()));
            CurRole.SetActive(true);
        }
        else
        {
            CurRole = GameObject.Instantiate(Resources.Load<GameObject>("Model/" + role.ToString()));
            CurRole.SetActive(true);
        }

        RoleTables[role] = CurRole;

        CurRole.transform.SetParent(PoseHelper.transform);
        CurRole.transform.localPosition = Vector3.zero;
        CurPlayerRoleModel = role;

        PoseHelper.Init(role);
    }

    private void S2C_UpdataPose(string param)
    {
        //Debug.Log(param);
        ZPose zp = JsonUtility.FromJson<ZPose>(param);
        //if (zp.role == CurPlayerRoleModel)
            PoseHelper.UpdataPose(zp);
    }

    #endregion



}
