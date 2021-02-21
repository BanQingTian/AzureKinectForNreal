using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZMain;

public class BarrierController : MonoBehaviour
{
    public enum MoveType
    {
        normal,
        accelerate,
        stop
    }

    public static BarrierController Instance;
    public GameObject icon_Prefab;
    public List<GameObject> iconList = new List<GameObject>();
    private GameData gameData;
    // 需重置
    [HideInInspector]
    public List<Vector3> pointList = new List<Vector3>();
    // 需重置游戏大逻辑
    [HideInInspector]
    public bool isStart = false;
    // 需重置游戏大逻辑
    private float timer = 0f;
    // 需重置
    private GameObject targetObj;
    // 需重置游戏大逻辑
    private List<GnerateData> gnerateListTemp = new List<GnerateData>();
    // 需重置游戏大逻辑
    private List<GnerateData> gnerateListTempOther = new List<GnerateData>();
    // 需重置游戏大逻辑
    private List<Barrier> moveList = new List<Barrier>();
    // 
    private MoveType moveType = MoveType.stop;
    //
    private int numCount = 0;
    //
    private int numCountCopy = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void CreateIcon(string name,int num)
    {
        switch (name)
        {
            case "Barrier_icon":
                for (int i = 0; i < num; i++)
                {
                    GameObject go = GameObject.Instantiate(icon_Prefab);
                    go.name = name;
                    go.transform.SetParent(transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localEulerAngles = new Vector3(0f,180f,0f);
                    go.SetActive(false);
                    iconList.Add(go);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitBarrierData(GameData data)
    {
        numCount = 0;
        targetObj = transform.Find("targetObj").gameObject;
        targetObj.transform.position = Vector3.zero;
        gameData = data;
        gnerateListTemp = gameData.gnerateConfig.gnerateList;
        foreach (GnerateData item in gnerateListTemp)
        {
            numCount += int.Parse(item.count);
            CreateIcon(item.name, int.Parse(item.count));
        }

        numCountCopy = numCount;
    }

    public List<GameObject> GetIdleElement(string name,int num)
    {
        List<GameObject> temp = new List<GameObject>();
        switch (name)
        {
            case "Barrier_icon":
                if (iconList.Count >= num)
                {
                    for (int i = 0; i < num; i++)
                    {
                        temp.Add(iconList[0]);
                        iconList.RemoveAt(0);
                    }
                }
                else
                {
                    Debug.LogError("金币池中对象不足");
                }
                break;
            default:
                break;
        }

        return temp;
    }

    private void Update()
    {
        if (isStart)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < gnerateListTemp.Count; i++)
            {
                if (timer >= float.Parse(gnerateListTemp[i].time))
                {
                    GatherPoint(GetSinPoint(gnerateListTemp[i].offsetx, gnerateListTemp[i].offsety, gnerateListTemp[i].offsetz,
                        gnerateListTemp[i].posx, gnerateListTemp[i].posy, gnerateListTemp[i].posz), gnerateListTemp[i]);
                }
            }
        }

        Move();
    }

    float zOffset = 0f;
    // 需重置
    float fixZ = 0f;
    // 需重置
    bool once = true;

    private Vector3 GetSinPoint(string x,string y,string z,string xPos,string yPos,string zPos)
    {
        if (fixZ == 0f)
        {
            fixZ = float.Parse(zPos);
        }

        if (once)
        {
            targetObj.transform.position = new Vector3(float.Parse(xPos), float.Parse(yPos), float.Parse(zPos));
            once = false;
        }
        else
        {
            zOffset += 1f;
            float sinValue = Mathf.Sin(zOffset * Mathf.Deg2Rad);
            targetObj.transform.position = new Vector3(targetObj.transform.position.x + sinValue * float.Parse(x)
                , targetObj.transform.position.y + sinValue * float.Parse(y), fixZ + zOffset * float.Parse(z));
        }

        return targetObj.transform.position;
    }

    int index = 0;
    private void GatherPoint(Vector3 pos, GnerateData data)
    {
        if (pointList.Count == 0)
        {
            pointList.Add(pos);
        }

        index += 1;
        if (index == ZMain.gatherRatio)
        {
            if (pointList.Count < int.Parse(data.count))
            {
                index = 0;
                pointList.Add(pos);
            }
            else
            {
                isStart = false;
                List<GameObject> temp = GetIdleElement(data.name,int.Parse(data.count));
                if (temp.Count < 0)
                {
                    return;
                }
                
                if (temp.Count > pointList.Count)
                {
                    int temp1 = temp.Count - pointList.Count;
                    for (int i = 0; i < temp1; i++)
                    {
                        temp.RemoveAt(temp.Count - 1 - i);
                    }
                }

                if (temp.Count < pointList.Count)
                {
                    int temp1 = pointList.Count - temp.Count;
                    for (int i = 0; i < temp1; i++)
                    {
                        pointList.RemoveAt(pointList.Count - 1 - i);
                    }
                }

                for (int i = 0; i < pointList.Count; i++)
                {
                    temp[i].transform.position = pointList[i];
                    temp[i].SetActive(true);
                    moveList.Add(temp[i].GetComponent<Barrier>());
                }
                gnerateListTempOther.Add(data);
                gnerateListTemp.Remove(data);
                ResetGatherPoint();
            }
        }
    }

    private void Move()
    {
        if (moveList.Count < 0)
        {
            return;
        }

        switch (moveType)
        {
            case MoveType.normal:
                foreach (Barrier item in moveList)
                {
                    item.speedTemp = 0.5f;//ZMain.moveSpeedMin;
                    if (GameManager.Instance.CurGameMode == GameMode.Drum)
                    {
                        StartGenerate();
                    }
                }
                break;
            case MoveType.accelerate:
                foreach (Barrier item in moveList)
                {
                    item.speedTemp = ZMain.moveSpeedMax;
                    if (GameManager.Instance.CurGameMode == GameMode.Drum)
                    {
                        //GameManager.Instance.isCanMove = true;
                        StartGenerate();
                    }
                }
                break;
            case MoveType.stop:
                foreach (Barrier item in moveList)
                {
                    item.speedTemp = 0f;
                    if (GameManager.Instance.CurGameMode == GameMode.Drum)
                    {
                        //GameManager.Instance.isCanMove = false;
                        isStart = false;
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ResetGatherPoint()
    {
        zOffset = 0f;
        index = 0;
        fixZ = 0f;
        once = true;
        targetObj.transform.position = Vector3.zero;
        pointList.Clear();
        StartGenerate();
    }

    // 外界调用控制金币重置
    public void AddIconList(GameObject go)
    {
        go.SetActive(false);
        iconList.Add(go);
        numCount -= 1;
        if (numCount <= 0)
        {
            ResetLogic();
        }
    }

    // 外界调用控制生成逻辑重置
    public void ResetLogic()
    {
        isStart = false;
        SetIconMoveType(MoveType.stop);
        numCount = numCountCopy;
        timer = 0f;
        gnerateListTemp.Clear();
        foreach (GnerateData item in gnerateListTempOther)
        {
            gnerateListTemp.Add(item);
        }
        gnerateListTempOther.Clear();
        zOffset = 0f;
        index = 0;
        fixZ = 0f;
        once = true;
        pointList.Clear();
        moveList.Clear();
    }

    // 外界调用控制生成逻辑重置
    public void TimeOutResetLogic()
    {
        //isStart = false;
        //SetIconMoveType(MoveType.stop);
        //numCount = numCountCopy;
        //timer = 0f;
        //gnerateListTemp.Clear();
        //foreach (GnerateData item in gnerateListTempOther)
        //{
        //    gnerateListTemp.Add(item);
        //}
        //gnerateListTempOther.Clear();
        //zOffset = 0f;
        //index = 0;
        //fixZ = 0f;
        //once = true;
        //pointList.Clear();
        //if (moveList.Count > 0)
        //{
        //    for (int i = 0; i < moveList.Count; i++)
        //    {
        //        AddIconList(moveList[i].gameObject);
        //    }
        //}
        //moveList.Clear();

        for (int i = 0; i < moveList.Count; i++)
        {
            Destroy(moveList[i]);
        }

        for (int i = 0; i < iconList.Count; i++)
        {
            Destroy(iconList[i]);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Barrier_icon")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        iconList.Clear();
        ResetLogic();

        gnerateListTemp.Clear();
        gnerateListTempOther.Clear();
        InitBarrierData(JsonMapper.ToObject<GameData>(GetLocalConfig("GameConfig")));
    }

    // 外界调用控制金币移动速度
    public void SetIconMoveType(MoveType mt)
    {
        moveType = mt;
    }

    // 外界调用控制物体生成
    public void StartGenerate()
    {
        isStart = true;
    }
}