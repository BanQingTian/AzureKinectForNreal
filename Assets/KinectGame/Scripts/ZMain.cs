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
    public class GameData
    {
        public GameConfig gameConfig;
        public GnerateConfig gnerateConfig;
    }

    public class GameConfig
    {
        public string ip;
        public string gameTime;
        public string gatherRatio;
        //public string moveSpeedMin;
        public string moveSpeed;
        public string sceneHight;
        public string forward;
        public string backward;
        public string rate;
    }

    public class GnerateConfig
    {
        public List<GnerateData> gnerateList = new List<GnerateData>();
    }

    public class GnerateData
    {
        public string name;
        public string count;
        public string posx;
        public string posy;
        public string posz;
        public string offsetx;
        public string offsety;
        public string offsetz;
        public string time;
    }

    private string ip = "192.168.1.111";
    public string gameData = "";
    public string generateTime = "";
    public static float gameTime = 0f;
    public static int gatherRatio = 0;
    //public static float moveSpeedMin = 0;
    public static float moveSpeedMax = 0f;
    public static float forward = 0f;
    public static float backward = 0f;
    public static float rate = 0f;
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
        // 获取ip 是否存在本地ip
        //ip = GetLocalIP(ip);
        // 获取游戏配置文件
        gameData = GetLocalConfig("GameConfig");
        if (gameData != "")
        {
            GameData gd = JsonMapper.ToObject<GameData>(gameData);
            ip = gd.gameConfig.ip;
            gameTime = float.Parse(gd.gameConfig.gameTime);
            gatherRatio = int.Parse(gd.gameConfig.gatherRatio);
            //moveSpeedMin = float.Parse(gd.gameConfig.moveSpeedMin);
            moveSpeedMax = float.Parse(gd.gameConfig.moveSpeed);
            forward = float.Parse(gd.gameConfig.forward);
            backward = float.Parse(gd.gameConfig.backward);
            rate = float.Parse(gd.gameConfig.rate);


            Transform sceneTran = GameManager.Instance.GameScene.transform.Find("Scene");
            sceneTran.transform.localPosition = new Vector3(sceneTran.transform.localPosition.x, 
                float.Parse(gd.gameConfig.sceneHight), sceneTran.transform.localPosition.z);

            BarrierController.Instance.InitBarrierData(gd);
        }
        
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
    public static string GetLocalConfig(string name)
    {
        string path;
#if UNITY_EDITOR
        path = Directory.GetParent(Application.dataPath).FullName + "/" + name + ".json";
#elif UNITY_ANDROID
        path = Application.persistentDataPath + "/" + name + ".json";
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
