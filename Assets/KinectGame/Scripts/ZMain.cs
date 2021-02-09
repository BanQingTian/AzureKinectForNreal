using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

/// <summary>
/// Game 入口
/// </summary>
public class ZMain : MonoBehaviour
{
    private string ip = "";
    public static string configData = "";
    private class JsonData
    {
        public JsonData(string x_data, string y_data, string z_data)
        {
            x = x_data;
            y = y_data;
            z = z_data;
        }

        public string x;
        public string y;
        public string z;
    }

    // Start is called before the first frame update
    void Start()
    {
        //初始化网络框架
        MessageManager.Instance.InitializeMessage();
        ip = "192.168.71.225";
        // 获取ip 是否存在本地ip
        ip = GetLocalIP(ip);
        // 连接sever
        MessageManager.Instance.SendConnectServerMsg(ip, "443");

#if UNITY_EDITOR
        GameManager.Instance.Init();
#else
        // 当成功加入房间后，初始化Game
        MessageManager.Instance.JoinRoomSuccessEvent += GameManager.Instance.Init;
#endif

        // 初始化私有协程
        ZCoroutiner.SetCoroutiner(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 5;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            Time.timeScale = 1;
        }
    }


    // 获取本地ip
    public static string GetLocalIP(string ip)
    {
        string path;
#if UNITY_EDITOR
        path = Directory.GetParent(Application.dataPath).FullName + "/IPAddress.txt";
#elif UNITY_ANDROID
        //path = "/storage/emulated/0/ABRes";
        path = Application.persistentDataPath + "/IPAddress.txt";
#endif
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        Debug.Log("ip ============ " + ip);
        return ip;
    }

    // 获取本地配置
    public static string GetLocalConfig()
    {
        string path;
#if UNITY_EDITOR
        path = Directory.GetParent(Application.dataPath).FullName + "/Config.txt";
#elif UNITY_ANDROID
        path = Application.persistentDataPath + "/Config.txt";
#endif
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            return "";
        }
    }
}
