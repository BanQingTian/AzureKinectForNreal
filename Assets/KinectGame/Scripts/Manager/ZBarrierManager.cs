//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ZBarrierManager : MonoBehaviour
//{
//    /// <summary>
//    /// 模型预制体
//    /// </summary>
//    public List<Barrier> BarriersModelPrefab = new List<Barrier>();
//    /// <summary>
//    /// 模型预制体字典
//    /// </summary>
//    public Dictionary<BarrierModelEnum, Barrier> BarrierModelDic = new Dictionary<BarrierModelEnum, Barrier>();

//    /// <summary>
//    /// 主要障碍物
//    /// </summary>
//    public List<Barrier> BarrierList = new List<Barrier>();

//    /// <summary>
//    /// 当前障碍物
//    /// </summary>
//    public Queue<Barrier> CurBarrierQueue = new Queue<Barrier>();

//    // barrier config
//    //List<BarrierTypeEnum> mBarrierTypeConfigList = new List<BarrierTypeEnum>()
//    //{
//    //    // 第一组 纯障碍物
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,

//    //    // 第二组 左右手+金币
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.LeftHand,BarrierTypeEnum.LeftHand,BarrierTypeEnum.RightHand,BarrierTypeEnum.Barrier,BarrierTypeEnum.RightHand,
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.Icon,BarrierTypeEnum.Barrier,BarrierTypeEnum.RightHand,BarrierTypeEnum.LeftHand,BarrierTypeEnum.Icon,

//    //    // 第三组 可摧毁+可拾取
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.Barrier,BarrierTypeEnum.CanPickUp,BarrierTypeEnum.CanPickUp, BarrierTypeEnum.CanPickUp,BarrierTypeEnum.CanPickUp,
//    //    BarrierTypeEnum.Barrier,BarrierTypeEnum.NeedDestroy,BarrierTypeEnum.Icon,BarrierTypeEnum.LeftHand,BarrierTypeEnum.LeftHand,BarrierTypeEnum.RightHand,
//    //};

//    // 对应配表的模型
//    List<BarrierModelEnum> mBarrierModelConfigList = new List<BarrierModelEnum>()
//    {
//        // 对应第一组的模型
//        BarrierModelEnum.Normal1,BarrierModelEnum.Normal1,BarrierModelEnum.Normal1,BarrierModelEnum.Normal1,BarrierModelEnum.Normal1,BarrierModelEnum.Normal1,
//        BarrierModelEnum.Normal2,BarrierModelEnum.Normal2,BarrierModelEnum.Normal2,BarrierModelEnum.Normal2,BarrierModelEnum.Normal2,BarrierModelEnum.Normal2,

//        // 对应第二组的模型
//        BarrierModelEnum.Normal2,BarrierModelEnum.LeftHand,BarrierModelEnum.LeftHand,BarrierModelEnum.RightHand,BarrierModelEnum.Normal1,BarrierModelEnum.RightHand,
//        BarrierModelEnum.Normal1,BarrierModelEnum.Icon1,BarrierModelEnum.Normal1,BarrierModelEnum.RightHand,BarrierModelEnum.LeftHand,BarrierModelEnum.Icon2,

//        // 对应第三组的模型
//        BarrierModelEnum.Normal1,BarrierModelEnum.Normal2,BarrierModelEnum.PickUp1,BarrierModelEnum.PickUp1,BarrierModelEnum.pickUp2,BarrierModelEnum.pickUp2,
//        BarrierModelEnum.Normal2,BarrierModelEnum.NeedDestroy1,BarrierModelEnum.Icon1,BarrierModelEnum.LeftHand,BarrierModelEnum.LeftHand,BarrierModelEnum.RightHand,
//    };

//    // 随机生成相关位置信息
//    private const float RandomGenMinX = -0.6f;
//    private const float RandomGenMaxX = 0.6f;
//    private const float RandomGenMinY = -1f;
//    private const float RandomGenMaxY = 0.09f;

//    // 必须摧毁障碍物位置信息
//    private const float DefaultGenPosX = 0f;
//    private const float DefaultGenPosY = -0.55f;
//    private const float DefaultGenPosZ = 11f;

//    // Z移动的范围
//    public static float StartMoveScopeZ = 11f;
//    public static float EndMoveScopeZ = 0f;

//    // 移动速度
//    public static float MoveSpeed = 1;
//    // 停止速度
//    public static float MoveStopSpeed = 1f;



//    /// <summary>
//    /// 加载障碍物
//    /// </summary>
//    /// <param name="FinishLaod"></param>
//    public void InitBarrierConfig(System.Action FinishLaod = null)
//    {
//        // 初始化预设字典
//        for (int i = 0; i < BarriersModelPrefab.Count; i++)
//        {
//            BarrierModelDic[BarriersModelPrefab[i].BarrierModel] = BarriersModelPrefab[i];
//        }


//        StartCoroutine(LoadBarrierModelFromConfig(FinishLaod));
//    }

//    bool isLoading = false;
//    private IEnumerator LoadBarrierModelFromConfig(System.Action Finish = null)
//    {
//        isLoading = true;

//        for (int i = 0; i < mBarrierModelConfigList.Count; i++)
//        {
//            BarrierList.Add(GetNewBarrier(mBarrierModelConfigList[i]));
//        }
//        yield return null;

//        isLoading = false;

//        Finish?.Invoke();
//    }

//    private Barrier GetNewBarrier(BarrierModelEnum bm)
//    {
//        return GameObject.Instantiate<Barrier>(BarrierModelDic[bm]);
//    }

//    /// <summary>
//    /// Run Barrier game
//    /// </summary>
//    public void BarrierCreator()
//    {
        
//        StartCoroutine(BarrierCreatorCor());
//    }

//    int curIndex = 0;
//    int totalIndex = 0;
//    Barrier curBarrier;
//    private IEnumerator BarrierCreatorCor()
//    {
//        Debug.Log("Loading Barrier...");
//        while (true)
//        {
//            if (totalIndex >= BarrierList.Count)
//            {
//                totalIndex = 0;
//                curIndex = 0;
//            }
//            if (curIndex >= 12)
//            {
//                curIndex = 0;
//                yield return new WaitForSeconds(1f);
//            }

//            yield return new WaitForSeconds(0.3f);

//            curBarrier = BarrierList[curIndex];
//            CurBarrierQueue.Enqueue(curBarrier);
//            curBarrier.Init(totalIndex);
//            BarrierPoseCreator(curBarrier);
//            curBarrier.Move();

//            curIndex++;
//            totalIndex++;
//        }
//    }

//    // 
//    private void BarrierPoseCreator(Barrier b)
//    {
//        switch (b.BarrierType)
//        {
//            case BarrierTypeEnum.Barrier:
//                NormalCreate(b.gameObject);
//                break;

//            case BarrierTypeEnum.Icon:
//                break;

//            case BarrierTypeEnum.LeftHand:
//                NormalCreate(b.gameObject);
//                break;

//            case BarrierTypeEnum.RightHand:
//                NormalCreate(b.gameObject);
//                break;

//            case BarrierTypeEnum.NeedDestroy:
//                break;

//            case BarrierTypeEnum.CanPickUp:
//                NormalCreate(b.gameObject);
//                break;

//            default:
//                break;
//        }
//    }

//    private void NormalCreate(GameObject go)
//    {
//        go.gameObject.transform.position = new Vector3(
//             Random.Range(RandomGenMinX, RandomGenMaxX)
//             , Random.Range(RandomGenMinY, RandomGenMaxY)
//             , StartMoveScopeZ);
//    }

//    public void BarrierMove()
//    {

//    }
//}
