using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BarrierData
{
    public BarrierInfo[] bp;
    // 总体的移动速度
    public float moveSpeed;
    // 一次的加载数量
    public int intervalCount;
    // 加载条件
    public int needLoadCount;

    public struct BarrierInfo
    {
        public int barrierType;
        public Vector3 barrierPos;
    }
}


public class ZBarrierManager : MonoBehaviour
{
    /// <summary>
    /// 障碍物模型预制体
    /// </summary>
    public List<Barrier> BarriersPrefab = new List<Barrier>();

    /// <summary>
    /// 障碍物模型预制体字典
    /// </summary>
    public Dictionary<BarrierTypeEnum, Barrier> BarriersPrefabDic = new Dictionary<BarrierTypeEnum, Barrier>();

    /// <summary>
    /// 当前存在的总数
    /// </summary>
    public static int CurTotalCount;

    /// <summary>
    /// 配表数据类
    /// </summary>
    public BarrierData mBarrierData;
    public BarrierData.BarrierInfo[] mBarrierInfo;

    public Transform BarrierParents;

    /// <summary>
    /// 移动速度
    /// </summary>
    private float MoveSpeed;

    /// <summary>
    /// 一次最多生成的数量
    /// </summary>
    private int IntervalCount;

    /// <summary>
    /// 低于这个数量是加载新的障碍物
    /// </summary>
    private int NeedLoadCount;

    /// <summary>
    /// 加载手机中的配置表
    /// </summary>
    /// <returns></returns>
    public string GetBarrierConfig()
    {
        // 从文件中加载数据
        string data = "";
        return data;
    }

    // 获取配表数据 
    private void UpdateBarrierData()
    {
        mBarrierData = JsonUtility.FromJson<BarrierData>(GetBarrierConfig());
        mBarrierInfo = mBarrierData.bp;
        MoveSpeed = mBarrierData.moveSpeed;
        IntervalCount = mBarrierData.intervalCount;
        NeedLoadCount = mBarrierData.needLoadCount;
    }

    /// <summary>
    /// 初始化配表数据
    /// </summary>
    public void InitConfig()
    {
        UpdateBarrierData();
        for (int i = 0; i < BarriersPrefab.Count; i++)
        {
            BarriersPrefabDic.Add(BarriersPrefab[i].BarrierType, BarriersPrefab[i]);
        }
    }

    /// <summary>
    /// 障碍物生成
    /// </summary>
    public void BarrierCreator()
    {
        StartCoroutine(CheckBarrierCount());
    }

    bool pause = false;
    bool stop = false;
    int curIndex = 0;
    private IEnumerator CheckBarrierCount()
    {
        while (true)
        {
            if (stop)
            {
                yield break;
            }
            if (CurTotalCount < NeedLoadCount)
            {
                
                loadBarrier(curIndex, IntervalCount);
            }

            yield return null;
        }
    }

    private void loadBarrier(int startIndex, int count)
    {
        if (startIndex >= mBarrierInfo.Length)
        {
            startIndex = 0;
        }

        count += startIndex + count;

        for (int i = startIndex; i < count; i++)
        {
            if(startIndex >= mBarrierInfo.Length)
            {
                startIndex = 0;
                break;
            }

            GetNewBarrier(mBarrierInfo[i].barrierType, mBarrierInfo[i].barrierPos);

            startIndex++;
            CurTotalCount++;
        }
    }

    // 获取障碍物
    private Barrier GetNewBarrier(int barrierType, Vector3 pos)
    {
        Barrier b = GameObject.Instantiate<Barrier>(BarriersPrefabDic[(BarrierTypeEnum)barrierType]);
        b.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);

        return b;
    }


}
