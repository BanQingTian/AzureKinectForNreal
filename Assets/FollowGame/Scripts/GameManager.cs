using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();


    public ZPoseHelper PoseHelper;
    public static bool Join = false;

    void Start()
    {
        Instance = this;
        S2CFuncTable.Add(S2CFuncName.PoseData, S2C_UpdataPose);
    }

    private void Update()
    {
        
    }

    private void S2C_UpdataPose(string param)
    {
        //Debug.Log(param);
        ZPose zp = JsonUtility.FromJson<ZPose>(param);
        PoseHelper.UpdataPose(zp);
    }

}
