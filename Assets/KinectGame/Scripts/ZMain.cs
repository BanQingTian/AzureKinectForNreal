using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MessageManager.Instance.InitializeMessage();
        string ip = "192.168.68.187";
        ip = GetLocalIP(ip);
        MessageManager.Instance.SendConnectServerMsg(ip, "443");

#if UNITY_EDITOR
        GameManager.Instance.Init();
#else
        MessageManager.Instance.JoinRoomSuccessEvent += GameManager.Instance.Init;
#endif

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
