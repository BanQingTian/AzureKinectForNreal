using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    private const string HandLeftTagName = "LeftHand";
    private const string HandRightTagName = "RightHand";
    private const string FootLeftTagName = "LeftFoot";
    private const string FootRightTagName = "RightFoot";
    private const string StakeboardPosName = "StakeboardPos";
    private const string StakeboardRotName = "StakeboardRot";

    private const float neckMoveLimit = 5f; // 脖子移动多少触发滑板移动
    private const float stakeboardMoveSpeed = 0.03f; // 滑板移动速度
    private const float stakeboardMoveLimit = 0.7f; //滑板移动上限

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
        Head = GameObject.Find("CenterCamera");
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

        GetAngle();

    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(Drum.gameObject);

        GameManager.Instance.PoseHelper.transform.localPosition = Vector3.zero;
    }

    float _headZ = 0;
    float _stakeboardX = 0;
    float moved = 0;
    private void StakeboardMove()
    {
        _headZ = Head.transform.rotation.eulerAngles.z;
        _headZ = _headZ > 300 ? _headZ - 360 : _headZ;
        moved = 0;
        if (_headZ > neckMoveLimit)
        {
            moved = (_headZ - neckMoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }
        else if (_headZ < -neckMoveLimit)
        {
            moved = (_headZ + neckMoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;
        }

        moved *= Mathf.Clamp(1 + Mathf.Abs(_headZ) - neckMoveLimit, 1, 1.1f);

        _stakeboardX = StakeboardPos.transform.position.x - moved;
        _stakeboardX = Mathf.Clamp(_stakeboardX, -stakeboardMoveLimit, stakeboardMoveLimit);
        StakeboardPos.transform.position = new Vector3(_stakeboardX, StakeboardPos.transform.position.y, StakeboardPos.transform.position.z);
    }

    Vector3 center;
    Vector3 dir;
    private void UpdateStakeboardDir()
    {
        center = (FootLeft.transform.position + FootRight.transform.position) / 2;
        dir = FootLeft.transform.position - FootRight.transform.position;
        Debug.DrawLine(FootLeft.transform.position, FootRight.transform.position, Color.yellow);
        GameManager.Instance.PoseHelper.transform.position = StakeboardRot.transform.position;// center;// new Vector3(Stakeboard.transform.position.x, FootLeft.transform.position.y - 0.01f, Stakeboard.transform.position.z);
        StakeboardRot.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        StakeboardRot.transform.localPosition = new Vector3(0, StakeboardRot.transform.localPosition.y, StakeboardRot.transform.localPosition.z);
        //StakeboardRot.transform.position = new Vector3(StakeboardRot.transform.position.x, StakeboardRot.transform.position.y, center.z);

    }

    private void GetAngle()
    {
        float angle = Vector3.Angle(a3.position - a2.position, a1.position - a2.position);
        angle = Mathf.Clamp(angle, 80, 170);
        float rate = (170 - angle) / 90;
        TempAnim.speed = 1 + rate;
    }
    Vector3 GetVerticalDir(Vector3 _dir)
    {
        //（_dir.x,_dir.z）与（？，1）垂直，则_dir.x * ？ + _dir.z * 1 = 0
        if (_dir.z == 0)
        {
            return new Vector3(0, 0, -1);
        }
        else
        {
            return new Vector3(-_dir.z / _dir.x, 0, 1).normalized;
        }
    }
}
