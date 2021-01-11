using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();

    public Dictionary<GameMode, ZGameBehaviour> GameTables = new Dictionary<GameMode, ZGameBehaviour>();

    // 任务姿态获取api
    public ZPoseHelper PoseHelper;

    // 是否加入游戏
    public static bool Join = false;

    public GameMode CurGameMode = GameMode.Football;
    public ZGameBehaviour CurGameBehaviour = null;


    void Start()
    {
        Instance = this;
        InitListener();
        InitGameBehaviour();

    }

    private void Update()
    {
        CurGameBehaviour.Update();
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            ChangeGameMode();
        }
    }

    #region Init

    public void InitListener()
    {
        S2CFuncTable.Add(S2CFuncName.PoseData, S2C_UpdataPose);
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
        }

        CurGameBehaviour.Start();
    }

    #endregion


    public void ChangeGameMode()
    {
        if(CurGameBehaviour!= null)
        {
            CurGameBehaviour.Release();
        }
        CurGameMode = CurGameMode == GameMode.Football ? GameMode.Drum : GameMode.Football;
        Debug.Log("ChangeGameMode.CurModel is "+ CurGameMode);
        InitGameBehaviour();
    }

    #region S2C

    private void S2C_UpdataPose(string param)
    {
        Debug.Log(param);
        ZPose zp = JsonUtility.FromJson<ZPose>(param);
        PoseHelper.UpdataPose(zp);
    }

    #endregion



}
