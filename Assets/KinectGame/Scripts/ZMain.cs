using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Game 入口
/// </summary>
public class ZMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //初始化网络框架
        MessageManager.Instance.InitializeMessage();
        string ip = "192.168.68.187";
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
        return ip;
    }
}
