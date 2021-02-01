using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //public void OnCollisionEnter(Collision collision)
    //{
    //    switch (GameManager.Instance.CurGameMode)
    //    {
    //        case GameMode.Football:

    //            if (!collision.collider.name.Contains("football")) return;

    //            float power = (_curPos - _lastPos).magnitude * 4222;
    //            //Vector3 dir = ((collision.transform.position - collision.contacts[0].point) - (_curPos - _lastPos)).normalized;

    //            Vector3 dir = (collision.transform.position - collision.contacts[0].point).normalized;

    //            dir = new Vector3(dir.x * 0.5f, 0.2f, Mathf.Abs(dir.z));

    //            collision.rigidbody.AddForce(dir * power);

    //            Debug.DrawRay(collision.contacts[0].point, dir * power * 0.004f, Color.yellow, 6);

    //            StartCoroutine(ResetFootballPos(collision.transform.GetComponent<Football>()));

    //            break;
    //        case GameMode.Drum:
    //            break;
    //        default:
    //            break;
    //    }
    //}


    private void FixedUpdate()
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

                if ((curRole = other.GetComponent<ZRole>()) != null) // model1
                {
                    if (changeRoleWaitingTime < 1.5f)
                        return;
                    changeRoleWaitingTime = 0;

                    GameManager.Instance.ChangePlayerRole(curRole.roleModel);
                }
                else if ((mainsound = other.GetComponent<ZSound>()) != null)
                {
                    if (changeBGSoundWaitingTime < 1.5f)
                        return;
                    changeBGSoundWaitingTime = 0;

                    GameManager.Instance.ChangeBgSound(mainsound.clip);
                }

                ActionTriggerEnter(other);

                break;

            //case GameMode.Football:

            //    break;
            case GameMode.Drum:

                barrierDetection(other);

                break;
            default:
                break;
        }
    }

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




    bool Pickuped = false;
    GameObject curFollower = null;

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


    bool isDoing = false;
    private IEnumerator ResetFootballPos(Football fb)
    {
        if (isDoing) yield break;
        isDoing = true;
        yield return new WaitForSeconds(6);
        isDoing = false;
        fb.transform.position = fb.defaultPos;
        fb.Rig.isKinematic = true;
        fb.Rig.isKinematic = false;
        fb.Rig.AddForce(new Vector3(1, 1, -0.5f));
    }

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
    private void delayFollow()
    {
        DelayGO.transform.position = Vector3.MoveTowards(DelayGO.transform.position, transform.position, 0.2f * Time.deltaTime);
    }
}
