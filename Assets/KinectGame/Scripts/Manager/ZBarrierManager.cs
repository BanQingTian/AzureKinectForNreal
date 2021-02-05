using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BarrierData
{
    public BarrierPose[] bp;
    public float startZ;
    public float moveSpeed;
    public float intervalTime;

    public struct BarrierPose
    {
        public int barrierType;
        public Vector2 barrierPos;
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
    /// 配表数据类
    /// </summary>
    public BarrierData mBarrierData;
    /// <summary>
    /// 障碍物起始z点
    /// </summary>
    public float StartZ;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float MoveSpeed;
    /// <summary>
    /// 生成的间隔时间
    /// </summary>
    public float IntervalTime;

    public string GetBarrierConfig()
    {
        string data = "";
        return data;
    }

    // 获取配表数据 
    private void UpdateBarrierData()
    {
        mBarrierData = JsonUtility.FromJson<BarrierData>(GetBarrierConfig());
        StartZ = mBarrierData.startZ;
        MoveSpeed = mBarrierData.moveSpeed;
        IntervalTime = mBarrierData.intervalTime;
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
        for (int i = 0; i < mBarrierData.bp.Length; i++)
        {

        }   
    }

    private IEnumerator BarrierCreatorCor()
    {
        int curIndex =0;
        while (true)
        {



            yield return new WaitForSeconds(IntervalTime);
            yield return null;
        }
    }

    // 获取障碍物
    private Barrier GetNewBarrier(int barrierType, Vector2 pos)
    {
        Barrier b = GameObject.Instantiate<Barrier>(BarriersPrefabDic[(BarrierTypeEnum)barrierType]);
        b.transform.localPosition = new Vector3(pos.x, pos.y, mBarrierData.startZ);
        return b;
    }

   
}
