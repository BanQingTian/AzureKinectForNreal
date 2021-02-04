using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    // 对应触碰节点的Tag标识，方便切换角色后的统一赋值
    private const string HandLeftTagName = "LeftHand";
    private const string HandRightTagName = "RightHand";
    private const string FootLeftTagName = "LeftFoot";
    private const string FootRightTagName = "RightFoot";
    private const string HipTagName = "Hip";

    // 角色脚下滑板的pose控制节点
    private const string StakeboardPosName = "StakeboardPos";
    private const string StakeboardRotName = "StakeboardRot";

    private const float MoveLimit = 2f; // 脖子移动多少触发滑板移动
    private const float stakeboardMoveSpeed = 0.07f; // 滑板移动速度
    private const float _humanXMoveLimit = 0.7f; //滑板移动上限
    private const float BarrierMoveSpeed = 1; // 障碍物移动速度
    private const float stopSpeed = 1f; // 障碍物停止的速度
    private const float frontMoveSpeed = 1.1f; //先前倾斜的速度
    private const float backMoveSpeed = 1.1f; // 向后倾斜的速度

    private const float head_hip_ratio = 0.7f; // 头和躯干造成移动的权重

    // 人物的默认起始x值，用于人物移动的范围的中心点
    public float _humanXDefaultX;

    // 模型左右手
    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    // 模型左右脚
    public GameObject FootLeft = null;
    public GameObject FootRight = null;

    // 用于计算膝盖位置的夹角而使用的三个节点
    private Transform a1, a2, a3; // for  a2->a1 与 a2->a3 的夹角,for 膝盖弯曲
    private Transform b1, b2, b3; // ditto

    // 控制障碍物的动画机
    private Animator TempAnim;

    // 滑板游戏的游戏场景
    public GameObject GameScene;

    // 眼镜节点的缓存，用于跳跃判断
    public GameObject Head; // for judge Move
    
    // 躯干的节点，用于控制人物移动
    public GameObject Hip;
    
    // 滑板的控制节点，position and rotation;
    public GameObject StakeboardPos;
    public GameObject StakeboardRot;

    /// <summary>
    /// 逻辑框架执行的起始
    /// </summary>
    public override void ZStart()
    {
        // 主要逻辑初始化人物模型上的数据节点


        HandLeft = GameObject.FindWithTag(HandLeftTagName);
        HandRight = GameObject.FindWithTag(HandRightTagName);

#if UNITY_EDITOR
        Head = GameObject.Find("Main");
#else
        Head = GameObject.Find("CenterCamera");
#endif
        Hip = GameObject.FindWithTag(HipTagName);
        FootLeft = GameObject.FindWithTag(FootLeftTagName);
        FootRight = GameObject.FindWithTag(FootRightTagName);

        a3 = FootLeft.transform.parent;
        a2 = a3.parent;
        a1 = a2.parent;

        b3 = FootRight.transform.parent;
        b2 = b3.parent;
        b1 = b2.parent;

        _humanXDefaultX = GameManager.Instance.PoseHelper.transform.position.x;

        if (HandLeft == null | HandRight == null)
        {
            Debug.LogError("Ex!!");
            return;
        }

        // 对应节点加载碰撞
        if (HandLeft.GetComponent<ZCollision>() == null)
        {
            var zc1 = HandLeft.AddComponent<ZCollision>();
            zc1.Init(CollisionTypeEnum.Hand);
            var zc2 = HandRight.AddComponent<ZCollision>();
            zc2.Init(CollisionTypeEnum.Hand);
        }

        ZDisplay();

        // new feature, change stake
        switch (GameManager.Instance.CurPlayerRoleModel)
        {
            case PlayerRoleModel.BlackGirl:
                GameManager.Instance.Stake_Aottman_gaming.SetActive(false);
                GameManager.Instance.Stake_Blackgirl_gaming.SetActive(true);
                break;
            case PlayerRoleModel.Aottman:
                GameManager.Instance.Stake_Aottman_gaming.SetActive(true);
                GameManager.Instance.Stake_Blackgirl_gaming.SetActive(false);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 逻辑框架的显示模块
    /// </summary>
    public override void ZDisplay(bool show = true)
    {
        base.ZDisplay(show);

        if (show)
        {
            if (GameScene == null)
            {
                GameScene = GameManager.Instance.GameScene;//GameObject.Instantiate(Resources.Load<GameObject>("Model/DrumPlus"));
                GameScene.SetActive(true);
                StakeboardPos = GameObject.Find(StakeboardPosName);
                StakeboardRot = GameObject.Find(StakeboardRotName);
                TempAnim = GameManager.Instance.GameAnim;//GameObject.Find("BarrierPart").GetComponent<Animator>();
                TempAnim.gameObject.SetActive(false);
            }
        }

        GameScene.SetActive(show);

        //HandLeft.GetComponent<Collider>().enabled = show;
        //HandRight.GetComponent<Collider>().enabled = show;
    }

    
    public override void ZPlay()
    {
        base.ZPlay();

        TempAnim.gameObject.SetActive(true);
    }


    /// <summary>
    /// 逻辑框架的update刷新
    /// </summary>
    public override void ZUpdate()
    {
        base.ZUpdate();

        StakeboardMove();

        UpdateStakeboardDir();

        StopMove();

        GetKneeAngle();

        //jumpDetection();

        jump2();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Editor_Jump");
            mainJump();
        }
#endif

        //Vector2 v = NRKernal.NRInput.GetTouch();
        //if (v == Vector2.zero) return;
        //if (v.x > 0.5f)
        //    jumpV += 0.01f;
        //else if (v.x < -0.5f)
        //    jumpV -= 0.01f;
        //UIManager.Instance.JumpV.text = jumpV.ToString(); ;

    }

    /// <summary>
    /// 逻辑框架的资源释放
    /// </summary>
    public override void ZRelease()
    {
        base.ZRelease();

        GameScene.SetActive(false);//GameObject.Destroy(Drum.gameObject);
        GameManager.Instance.PoseHelper.transform.localPosition = Vector3.zero;
        foreach (Transform item in GameManager.Instance.WallParent)
        {
            item.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 滑板左右移动逻辑 
    /// </summary>
    float _headZ = 0; // 头在z轴的偏移量
    float _hipZ = 0;  // 胯，躯干在z轴的偏移量
    float _humanX = 0; // 滑板移动的位置
    float moved = 0; // 权重计算之后的偏移量
    float moved_head = 0; // 头部的偏移量
    float moved_hip = 0; // 胯躯干的偏移量
    float leftOrRight_front = 0; // 大于0 left在前 反之
    private void StakeboardMove()
    {
        leftOrRight_front = FootLeft.transform.position.z - FootLeft.transform.position.z;

        // 头部倾斜计算
        _headZ = Head.transform.rotation.eulerAngles.z;
        _headZ = _headZ > 300 ? _headZ - 360 : _headZ;
        moved_head = 0;
        if (_headZ > MoveLimit)
        {
            moved_head = (_headZ - MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
            moved_head = leftOrRight_front > 0 ? moved_head * backMoveSpeed : moved_head * frontMoveSpeed;
        }
        else if (_headZ < -MoveLimit)
        {
            moved_head = (_headZ + MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
            moved_head = leftOrRight_front > 0 ? moved_head * backMoveSpeed : moved_head * frontMoveSpeed;
        }
        moved_head *= Mathf.Clamp(1 + Mathf.Abs(_headZ) - MoveLimit, 1, 1.2f);


        // 身体倾斜计算
        _hipZ = Hip.transform.rotation.eulerAngles.z;
        _hipZ = _hipZ > 300 ? _hipZ - 360 : _hipZ;
        moved_hip = 0;
        if (_hipZ > MoveLimit)
        {
            moved_hip = (_hipZ - MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
            moved_hip = leftOrRight_front > 0 ? moved_hip * backMoveSpeed : moved_hip * frontMoveSpeed;
        }
        else if (_hipZ < -MoveLimit)
        {
            moved_hip = (_hipZ + MoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
            moved_hip = leftOrRight_front > 0 ? moved_hip * backMoveSpeed : moved_hip * frontMoveSpeed;
        }
        moved_hip *= Mathf.Clamp(1 + Mathf.Abs(_hipZ) - MoveLimit, 1, 1.1f);


        // 综合权重
        moved = moved_head * head_hip_ratio + moved_hip * (1 - head_hip_ratio);

        // 通过移动人父物体移动，后续让滑板跟随人的位置
        _humanX = GameManager.Instance.PoseHelper.transform.position.x - moved;
        _humanX = Mathf.Clamp(_humanX, -_humanXMoveLimit + _humanXDefaultX, _humanXMoveLimit + _humanXDefaultX);
        GameManager.Instance.PoseHelper.transform.position = new Vector3(_humanX
            , GameManager.Instance.PoseHelper.transform.position.y
            , GameManager.Instance.PoseHelper.transform.position.z);
        //StakeboardPos.transform.position = new Vector3(_stakeboardX, StakeboardPos.transform.position.y, StakeboardPos.transform.position.z);
    }


    /// <summary>
    /// 滑板的方向刷新，方向根据两脚的方向
    /// </summary>
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

    float curAnimSpeed;
    private IEnumerator stopMoveCor()
    {
        curAnimSpeed = TempAnim.speed;
        while (TempAnim.speed > 0)
        {
            TempAnim.speed -= stopSpeed * Time.fixedDeltaTime * BarrierMoveSpeed;
            yield return null;
        }

        GameManager.wallMoveaSpeed = 0;
        GameManager.wallCreateTime = 0;

        TempAnim.speed = 0;
    }


    /// <summary>
    /// 弯曲膝盖 加速（目前加速使用Animator.speed控制
    /// </summary>
    private void GetKneeAngle()
    {
        if (beginStopMove) return;

        if (TempAnim.speed < curAnimSpeed)
        {
            TempAnim.speed += stopSpeed * Time.fixedDeltaTime;
        }
        float angle = GetLeftAngle();
        float rate = (170 - angle) / 90;
        TempAnim.speed = BarrierMoveSpeed + rate;

        GameManager.wallMoveaSpeed = TempAnim.speed;
        GameManager.wallCreateTime = 1.5f / TempAnim.speed;
    }




    /// <summary>
    /// 跳跃检测  --------- Invalid
    /// </summary>
    float _curLeft;
    float _lastLeft;
    float _curRight;
    float _lastRight;
    float _countdown = 0;
    bool detectioning = false;
    bool jumpSuccess = false;
    float startCurveAngle = 140;// 开始计算弯曲的角度
    float curveAngle = 15; // 弯曲有效的时间
    float curveValidTime = 0.25f; // 弯曲时间
    private void jumpDetection()
    {
        // left
        if (GetLeftAngle() < startCurveAngle && GetRightAngle() < startCurveAngle) // 正在弯曲膝盖
        {
            _lastLeft = GetLeftAngle();
            _lastRight = GetRightAngle();
            detectioning = true;
            _countdown = 0;
        }
        if (detectioning)
        {
            _countdown += Time.fixedDeltaTime;
            if (_countdown > curveValidTime)
            {
                _curLeft = GetLeftAngle();
                _curRight = GetRightAngle();
                if (_curLeft - _lastLeft > curveAngle && _curRight - _lastRight > curveAngle)
                {
                    Debug.Log("jump");
                    mainJump();
                }
                detectioning = false;
                _countdown = 0;
            }

        }

    }
    private float GetLeftAngle()
    {
        float angle = Vector3.Angle(a3.position - a2.position, a1.position - a2.position);
        return Mathf.Clamp(angle, 80, 170);
    }
    private float GetRightAngle()
    {
        float angle = Vector3.Angle(b3.position - b2.position, b1.position - b2.position);
        return Mathf.Clamp(angle, 80, 170);
    }




    /// <summary>
    /// 跳跃检测 -- （因为kinect对跳跃检测不明显，所有代码加大跳跃幅度
    /// 根据头的位置和膝盖弯曲角度判断跳跃
    /// </summary>
    float _upV;
    Vector3 _curPos;
    Vector3 _lastPos;
    float jumpV = 0.06f;
    float jumpInterval = 0.65f;
    public void jump2()
    {
        jumpInterval += Time.deltaTime;
        if (_lastPos == Vector3.zero)
        {
            _lastPos = GameManager.Instance.head.position;
            return;
        }
        _curPos = GameManager.Instance.head.position;

        if (jumpInterval > 0.65f)
        {
            if (_curPos.y - _lastPos.y > 0.06f || _curPos.y > GameManager.Instance.HeadStartPos.y + 0.005f)
            {
                if (GetLeftAngle() < startCurveAngle && GetRightAngle() < startCurveAngle)
                {
                    Debug.Log("jump");
                    mainJump();
                    _lastPos = Vector3.zero;
                    jumpInterval = 0;
                }
            }
        }
    }

    private void mainJump()
    {
        var j = GameManager.Instance.PoseHelper.GetComponent<RoleJump>();
        if (j == null)
        {
            j = GameManager.Instance.PoseHelper.gameObject.AddComponent<RoleJump>();
        }
        j.Init(GameManager.Instance.Groud.y);
    }
}
