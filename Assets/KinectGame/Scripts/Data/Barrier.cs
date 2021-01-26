using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Vector3 DefalutPos;

    //public int Index;
    // 障碍物类型
    public BarrierTypeEnum BarrierType;
    // 障碍物模型类型
    public BarrierModelEnum BarrierModel;
    // 障碍物特效
    public SpecialEffectEnum SpecialEff;

    public SoundEffEnum SoundEff;

    private void OnEnable()
    {
        DefalutPos = transform.localPosition;
    }

    /// <summary>
    /// 播放特效，音频等资源
    /// </summary>
    public void Play()
    {

        switch (BarrierType)
        {
            case BarrierTypeEnum.Barrier:
                break;
            case BarrierTypeEnum.Icon:
                gameObject.SetActive(false);
                break;

            case BarrierTypeEnum.LeftHand:
                gameObject.SetActive(false);
                break;

            case BarrierTypeEnum.RightHand:
                gameObject.SetActive(false);
                break;

            case BarrierTypeEnum.NeedDestroy:
                gameObject.SetActive(false);
                break;

            case BarrierTypeEnum.CanPickUp:
                gameObject.SetActive(false);

                break;
            default:
                break;
        }
        // load from Res,,, todo
        Debug.Log("Play Eff");
    }


























    /* ------------------------------invalid-------------------------------------- 

    private float curMoveSpeed;

    bool move = false;

    public void Init(int i)
    {
        Index = i;
        gameObject.SetActive(true);
        move = false;
    }

    // 开始移动
    public void Move()
    {
        StartCoroutine(moveCor());
        move = true;
    }

    private IEnumerator moveCor()
    {
        while (true)
        {
            if (transform.position.z <= ZBarrierManager.EndMoveScopeZ)
            {
                move = false;
                gameObject.SetActive(false);
                //if (GameManager.Instance.BarrierMananger.CurBarrierQueue.Peek().Index == Index)
                    GameManager.Instance.BarrierMananger.CurBarrierQueue.Dequeue();
                yield break;
            }

            transform.position += Vector3.forward * ZBarrierManager.MoveSpeed * Time.fixedDeltaTime;

            yield return null;
        }
    }

    // 停止运动
    public void StopSlow()
    {
        StartCoroutine(stopSlowCor());
    }
    private IEnumerator stopSlowCor()
    {
        float ms = ZBarrierManager.MoveSpeed;
        while (true)
        {
            if (transform.position.z <= ZBarrierManager.EndMoveScopeZ)
            {
                move = false;
                gameObject.SetActive(false);
                yield break;
            }

            ms = ms - ZBarrierManager.MoveStopSpeed / 30;
            if (ms <= 0)
            {
                move = false;
                yield break;
            }
            transform.position += -Vector3.forward * ms * Time.fixedDeltaTime;
        }
    }

    */
}
