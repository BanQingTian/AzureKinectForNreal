using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 主要碰撞逻辑检测
/// </summary>
public class ZCollision : MonoBehaviour
{
    protected bool ddd = true;
    private Vector3 _lastPos;
    private Vector3 _curPos;

    public GameObject DelayGO;

    public CollisionTypeEnum CollisionType;

    public void Init(CollisionTypeEnum ct)
    {
        CollisionType = ct;
    }

    private void FixedUpdate()
    {
        ThrowBarrier();  
    }

    // 扔手上的障碍物
    private void ThrowBarrier()
    {
        // update data
        _lastPos = _curPos;
        _curPos = transform.position;

        changeBGSoundWaitingTime += Time.fixedDeltaTime;
        changeRoleWaitingTime += Time.fixedDeltaTime;

        // 扔出物体
        if (Pickuped)
        {
            if (curFollower.transform.position.z - DelayGO.transform.position.z > 0.15f)
            {
                Pickuped = false;
                curFollower.AddComponent<ShootForward>();
                curFollower = null;

            }
        }
        if (follow)
        {
            delayFollow();
        }
    }

    float changeRoleWaitingTime = 1.5f;
    float changeBGSoundWaitingTime = 1.5f;
    ZRole curRole;
    ZSound mainsound;
    public void OnTriggerEnter(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Prepare:
                if (CollisionType == CollisionTypeEnum.Hand)
                {
                    // 切换角色
                    if ((curRole = other.GetComponent<ZRole>()) != null) // model1
                    {
                        if (changeRoleWaitingTime < 1.5f)
                            return;

                        changeRoleWaitingTime = 0;
                        GameManager.Instance.ChangePlayerRole(curRole.roleModel);
                    }
                    // 切换声音 ---- Invalid
                    else if ((mainsound = other.GetComponent<ZSound>()) != null)
                    {
                        if (changeBGSoundWaitingTime < 1.5f)
                            return;
                        changeBGSoundWaitingTime = 0;

                        GameManager.Instance.ChangeBgSound(mainsound.clip);
                    }

                    ActionTriggerEnter(other);
                }

                break;

            case GameMode.Drum:

                barrierDetection(other);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 障碍物碰撞检测
    /// </summary>
    bool Pickuped = false;
    GameObject curFollower = null;
    private void barrierDetection(Collider other)
    {
        var barrier = other.GetComponent<Barrier>();
        if (barrier != null && ddd)
        {
            if (barrier.isHold) return;
            ddd = true;
            switch (barrier.BarrierType)
            {
                case BarrierTypeEnum.Barrier:
                    barrier.Play();
                    GameManager.Instance.SetScore(ZConstant.BarrierScore);
                    break;

                case BarrierTypeEnum.Icon:
                    barrier.Play();
                    GameManager.Instance.SetScore(ZConstant.IconScore);
                    break;

                case BarrierTypeEnum.LeftHand:
                    if (CollisionType == CollisionTypeEnum.Hand)
                    {
                        GameManager.Instance.SetScore(ZConstant.HandScore);
                    }
                    barrier.Play();
                    break;

                case BarrierTypeEnum.RightHand:
                    barrier.Play();
                    if (CollisionType == CollisionTypeEnum.Hand)
                        GameManager.Instance.SetScore(ZConstant.HandScore);
                    break;

                case BarrierTypeEnum.NeedDestroy:
                    barrier.Play();
                    GameManager.Instance.SetScore(ZConstant.DotnDestroyWallScore);
                    break;

                case BarrierTypeEnum.CanPickUp:

                    if (CollisionType != CollisionTypeEnum.Hand)
                    {
                        barrier.Play();
                        return;
                    }

                    if (!Pickuped)
                    {
                        Pickuped = true;

                        //barrier.transform.SetParent(transform);
                        //barrier.transform.localPosition = Vector3.zero;
                        //barrier.GetComponent<Collider>().enabled = false;
                        barrier.Play();
                        var follower = Instantiate<GameObject>(barrier.gameObject);
                        var r = follower.AddComponent<Rigidbody>();
                        r.useGravity = false;
                        r.isKinematic = true;
                        follower.transform.position = transform.position;
                        follower.transform.SetParent(transform);
                        follower.transform.localScale = Vector3.one;
                        follower.SetActive(true);
                        follower.GetComponent<Barrier>().isHold = true;
                        ResetDelayFollow();
                        curFollower = follower;
                    }

                    break;

                default:
                    break;
            }
        }
    }


    public void OnTriggerExit(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Prepare:
                ActionTriggerExit(other);
                break;
            //case GameMode.Football:
            //    break;
            case GameMode.Drum:
                ddd = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 角色切换Trigger Enter
    /// </summary>
    private void ActionTriggerEnter(Collider other)
    {
        switch (CollisionType)
        {
            case CollisionTypeEnum.Foot:
                break;
            case CollisionTypeEnum.Hand:
                if (other.name.Equals("A1"))
                {
                    GameManager.Instance.A1Hover = true;
                    Debug.Log("GameManager.Instance.A1Hover = true;");
                }
                if (other.name.Equals("A2"))
                {
                    GameManager.Instance.A2Hover = true;
                    Debug.Log("GameManager.Instance.A2Hover = true;");
                }
                break;
            case CollisionTypeEnum.Hip:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 角色切换Trigger Exit
    /// </summary>
    private void ActionTriggerExit(Collider other)
    {
        switch (CollisionType)
        {
            case CollisionTypeEnum.Foot:
                break;
            case CollisionTypeEnum.Hand:
                if (other.name.Equals("A1"))
                {
                    GameManager.Instance.A1Hover = false;
                    Debug.Log("GameManager.Instance.A1Hover = false;");
                }
                if (other.name.Equals("A2"))
                {
                    GameManager.Instance.A2Hover = false;
                    Debug.Log("GameManager.Instance.A2Hover = false;");
                }
                break;
            case CollisionTypeEnum.Hip:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 重置扔东西逻辑的判断节点
    /// </summary>
    bool follow = false;
    private void ResetDelayFollow()
    {
        if (DelayGO == null)
        {
            DelayGO = new GameObject("delay_go");
        }
        DelayGO.transform.position = transform.position;
        follow = true;
    }
    /// <summary>
    /// 扔东西判断节点位置更新
    /// </summary>
    private void delayFollow()
    {
        DelayGO.transform.position = Vector3.MoveTowards(DelayGO.transform.position, transform.position, 0.18f * Time.deltaTime);
    }
}
