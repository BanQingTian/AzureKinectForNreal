using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    private const string HandLeftTagName = "LeftHand";
    private const string HandRightTagName = "RightHand";
    private const string FootLeftTagName = "LeftFoot";
    private const string FootRightTagName = "RightFoot";
    private const string HipTagName = "Hip";

    private const string StakeboardPosName = "StakeboardPos";
    private const string StakeboardRotName = "StakeboardRot";

    private const float MoveLimit = 2f; // 脖子移动多少触发滑板移动
    private const float stakeboardMoveSpeed = 0.04f; // 滑板移动速度
    private const float stakeboardMoveLimit = 0.7f; //滑板移动上限
    private const float stopSpeed = 1f; // 障碍物停止的速度

    private const float head_hip_ratio = 0.5f; // 头和躯干造成移动的权重

    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    public GameObject FootLeft = null;
    public GameObject FootRight = null;

    private Transform a1, a2, a3; // for  a2->a1 与 a2->a3 的夹角

    private Animator TempAnim;

    public GameObject Drum;
    public GameObject DrumPlusScene;

    public GameObject Head; // for judge Move
    public GameObject Hip;
    public GameObject StakeboardPos;
    public GameObject StakeboardRot;

    private Vector3 DefaultPos;

    public override void ZStart()
    {
        HandLeft = GameObject.FindWithTag(HandLeftTagName);
        HandRight = GameObject.FindWithTag(HandRightTagName);

#if UNITY_EDITOR
        Head = GameObject.Find("Main");
#else
        Head = GameObject.Find("CenterCamera");
#endif
        Hip = GameObject.FindWithTag(HipTagName);
        Debug.Log(Hip.name + "-----------");
        FootLeft = GameObject.FindWithTag(FootLeftTagName);
        FootRight = GameObject.FindWithTag(FootRightTagName);


        a3 = FootLeft.transform.parent;
        a2 = a3.parent;
        a1 = a2.parent;

        if (HandLeft == null | HandRight == null)
        {
            Debug.LogError("Ex!!");
            return;
        }

        if (HandLeft.GetComponent<ZCollision>() == null)
        {
            HandLeft.AddComponent<ZCollision>();
            HandRight.AddComponent<ZCollision>();
        }

        ZDisplay();
    }

    public override void ZDisplay(bool show = true)
    {
        base.ZDisplay(show);

        if (show)
        {
            if (Drum == null)
            {
                Drum = GameObject.Instantiate(Resources.Load<GameObject>("Model/DrumPlus"));
                StakeboardPos = GameObject.Find(StakeboardPosName);
                StakeboardRot = GameObject.Find(StakeboardRotName);
                TempAnim = GameObject.Find("random_flying_cube").GetComponent<Animator>();
            }
        }

        Drum.SetActive(show);

        HandLeft.GetComponent<Collider>().enabled = show;
        HandRight.GetComponent<Collider>().enabled = show;
    }

    public override void ZUpdate()
    {
        base.ZUpdate();

        StakeboardMove();

        UpdateStakeboardDir();

        StopMove();

        GetKneeAngle();

    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(Drum.gameObject);

        GameManager.Instance.PoseHelper.transform.localPosition = Vector3.zero;
    }

    float _headZ = 0; // 头在z轴的偏移量
    float _hipZ = 0;  // 胯，躯干在z轴的偏移量
    float _stakeboardX = 0; // 滑板移动的位置
    float moved = 0; // 权重计算之后的偏移量
    float moved_head = 0; // 头部的偏移量
    float moved_hip = 0; // 胯躯干的偏移量
    private void StakeboardMove()
    {
        _headZ = Head.transform.rotation.eulerAngles.z;
        _headZ = _headZ > 300 ? _headZ - 360 : _headZ;
        moved_head = 0;
        if (_headZ > MoveLimit)
        {
            moved_head = (_headZ - MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }
        else if (_headZ < -MoveLimit)
        {
            moved_head = (_headZ + MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }
        moved_head *= Mathf.Clamp(1 + Mathf.Abs(_headZ) - MoveLimit, 1, 1.1f);

        _hipZ = Hip.transform.rotation.eulerAngles.z;
        Debug.Log(_hipZ);
        _hipZ = _hipZ > 300 ? _hipZ - 360 : _hipZ;
        moved_hip = 0;
        if (_hipZ > MoveLimit)
        {
            moved_hip = (_hipZ - MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }
        else if (_hipZ < -MoveLimit)
        {
            moved_hip = (_hipZ + MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }
        moved_hip *= Mathf.Clamp(1 + Mathf.Abs(_hipZ) - MoveLimit, 1, 1.1f);


        moved = moved_head * head_hip_ratio + moved_hip * (1 - head_hip_ratio);

        _stakeboardX = GameManager.Instance.PoseHelper.transform.position.x - moved;
        _stakeboardX = Mathf.Clamp(_stakeboardX, -stakeboardMoveLimit, stakeboardMoveLimit);

        GameManager.Instance.PoseHelper.transform.position = new Vector3(_stakeboardX
            , GameManager.Instance.PoseHelper.transform.position.y
            , GameManager.Instance.PoseHelper.transform.position.z);
        //StakeboardPos.transform.position = new Vector3(_stakeboardX, StakeboardPos.transform.position.y, StakeboardPos.transform.position.z);
    }

    Vector3 center;
    Vector3 dir;
    private void UpdateStakeboardDir()
    {
        center = (FootLeft.transform.position + FootRight.transform.position) / 2;
        dir = FootLeft.transform.position - FootRight.transform.position;
        Debug.DrawLine(FootLeft.transform.position, FootRight.transform.position, Color.yellow);

        //GameManager.Instance.PoseHelper.transform.position = StakeboardRot.transform.position;// center;// new Vector3(Stakeboard.transform.position.x, FootLeft.transform.position.y - 0.01f, Stakeboard.transform.position.z);
        StakeboardPos.transform.position = center;// GameManager.Instance.PoseHelper.transform.position;

        StakeboardRot.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        StakeboardRot.transform.localPosition = new Vector3(0, StakeboardRot.transform.localPosition.y, StakeboardRot.transform.localPosition.z);
        //StakeboardRot.transform.position = new Vector3(StakeboardRot.transform.position.x, StakeboardRot.transform.position.y, center.z);

    }

    Coroutine cor = null;
    bool beginStopMove = false;
    float ea;
    private void StopMove()
    {
        ea = (Mathf.Abs(StakeboardRot.transform.localRotation.eulerAngles.y) + 90) % 180;
        if (ea < 20 || ea > 160)
        {
            if (!beginStopMove)
            {
                Debug.Log(2222222);
                cor = ZCoroutiner.StartCoroutine(stopMoveCor());
                beginStopMove = true;
            }
        }
        else
        {
            if (beginStopMove)
            {
                ZCoroutiner.StopCoroutine(cor);
                beginStopMove = false;
            }
        }
    }


    private IEnumerator stopMoveCor()
    {
        while (TempAnim.speed > 0)
        {
            TempAnim.speed -= stopSpeed * Time.fixedDeltaTime;
            yield return null;
        }
        TempAnim.speed = 0;
        Debug.Log(3333333);

    }

    private void GetKneeAngle()
    {
        if (beginStopMove) return;

        float angle = Vector3.Angle(a3.position - a2.position, a1.position - a2.position);
        angle = Mathf.Clamp(angle, 80, 170);
        float rate = (170 - angle) / 90;
        TempAnim.speed = 1 + rate;
    }

}
