﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    /// <summary>
    /// 该障碍物是否被抓住
    /// </summary>
    public bool isHold = false;
    private bool isCanMove = false;
    public bool canMove
    {
        get
        {
            return isCanMove;
        }

        set
        {
            isCanMove = value;
        }
    }

    //public int Index;
    // 障碍物类型
    public BarrierTypeEnum BarrierType;
    // 障碍物模型类型
    public BarrierModelEnum BarrierModel;
    // 障碍物特效
    public SpecialEffectEnum SpecialEff;
    // 音频类型
    public SoundEffEnum SoundEff;

    private float speed = 0f;
    public float speedTemp
    {
        get { return speed; }
        set { speed = value; }

    }
    public GameObject obj;


    /// <summary>
    /// 播放特效，音频等资源
    /// </summary>
    public void Play()
    {
        gameObject.SetActive(false);
        BarrierController.Instance.AddIconList(gameObject);

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
        if(SpecialEff == SpecialEffectEnum.Coin)
        {
            eff.transform.position += Vector3.up * 0.061f;
        }
        eff.transform.rotation = Quaternion.identity;
        eff.GetComponent<ParticleSystem>().Play();
    }

    public void SetPostion(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        if (canMove)
        {
            if (GameManager.Instance.isCanMove)
            {
                transform.position += transform.forward * Time.deltaTime * ZMain.moveSpeedMax;
            }
        }

        //zOffset += 1f;
        //float sinValue = Mathf.Sin(zOffset * Mathf.Deg2Rad);
        //Debug.LogError("sinValue ==== " + sinValue);
        //transform.position = new Vector3(transform.position.x + sinValue * 0.02f, transform.position.y + sinValue * 0.02f, ddd + zOffset * 0.02f);
        //GameObject.Instantiate(obj,transform.position,Quaternion.identity);
        if (transform.position.z <= -2f)
        {
            BarrierController.Instance.AddIconList(gameObject);
        }
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
