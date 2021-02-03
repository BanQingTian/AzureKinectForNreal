using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public bool isHold = false;

    [Header("Is Aottaman")]
    public bool isAottaman = false;

    public Vector3 DefalutScale;

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
        DefalutScale = transform.lossyScale;
    }

    /// <summary>
    /// 播放特效，音频等资源
    /// </summary>
    public void Play()
    {
        gameObject.SetActive(false);

        GameManager.Instance.PlayBarrierAudio(SoundEff, transform.position);

        switch (BarrierType)
        {
            case BarrierTypeEnum.Barrier:
                break;

            case BarrierTypeEnum.Icon:
                break;

            case BarrierTypeEnum.LeftHand:
                break;

            case BarrierTypeEnum.RightHand:
                break;

            case BarrierTypeEnum.NeedDestroy:
                break;

            case BarrierTypeEnum.CanPickUp:
                break;

            default:
                break;
        }

        // load from Res,,, todo
        Debug.Log("Play Eff");
        var eff = GameResConfig.Instance.GetSpecialEff(SpecialEff);
        eff.transform.position = gameObject.transform.position;
        eff.transform.rotation = Quaternion.identity;
        eff.GetComponent<ParticleSystem>().Play();


        //GameResConfig.Instance.GetAudioEff(SoundEff);
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
