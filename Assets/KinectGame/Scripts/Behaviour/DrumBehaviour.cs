using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    private const string HandLeftTagName = "LeftHand";
    private const string HandRightTagName = "RightHand";
    private const string FootLeftTagName = "LeftFoot";
    private const string FootRightTagName = "RightFoot";
    private const string NeckTagName = "Neck";
    private const string StakeboardName = "Stakeboard";

    private const float neckMoveLimit = 1f; // 脖子移动多少触发滑板移动
    private const float stakeboardMoveSpeed = 0.2f; // 滑板移动速度
    private const float stakeboardMoveLimit = 0.7f; //滑板移动上限

    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    public GameObject FootLeft = null;
    public GameObject FootRight = null;

    private Transform a1, a2, a3; // for  a2->a1 与 a2->a3 的夹角

    private Animator TempAnim;

    public GameObject Drum;
    public GameObject DrumPlusScene;

    public GameObject Neck; // for judge Move
    public GameObject Stakeboard;

    private Vector3 DefaultPos;

    public override void ZStart()
    {
        HandLeft = GameObject.FindWithTag(HandLeftTagName);
        HandRight = GameObject.FindWithTag(HandRightTagName);
        Neck = GameObject.FindWithTag(NeckTagName);
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
                Stakeboard = GameObject.Find(StakeboardName);
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

    float _neckX = 0;
    float _stakeboardX = 0;
    float moved = 0;
    private void StakeboardMove()
    {
        _neckX = Neck.transform.localRotation.eulerAngles.z;
        _neckX = _neckX > 300 ? _neckX - 360 : _neckX;
        if (_neckX > neckMoveLimit || _neckX < -neckMoveLimit)
        {
            moved = (_neckX - neckMoveLimit) * stakeboardMoveSpeed * Time.fixedDeltaTime;// * (1 + Mathf.Abs(_neckX - neckMoveLimit));
        }
        _stakeboardX = Stakeboard.transform.localPosition.x - moved;

        _stakeboardX = Mathf.Clamp(_stakeboardX, -stakeboardMoveLimit, stakeboardMoveLimit);

        Stakeboard.transform.localPosition = new Vector3(_stakeboardX, Stakeboard.transform.localPosition.y, Stakeboard.transform.localPosition.z);

        GameManager.Instance.PoseHelper.transform.position = new Vector3(Stakeboard.transform.position.x
            , GameManager.Instance.PoseHelper.transform.position.y
            , GameManager.Instance.PoseHelper.transform.position.z);
    }

    private void UpdateStakeboardDir()
    {
        Vector3 center = (FootLeft.transform.position + FootRight.transform.position) / 2;
        Vector3 dir = (FootLeft.transform.position - FootRight.transform.position).normalized;
        Debug.DrawLine(FootLeft.transform.position, FootRight.transform.position, Color.yellow);
        Stakeboard.transform.position = new Vector3(Stakeboard.transform.position.x, FootLeft.transform.position.y - 0.01f, Stakeboard.transform.position.z);
        Stakeboard.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 angle = Stakeboard.transform.rotation.eulerAngles;
        Stakeboard.transform.rotation = Quaternion.Euler(0, angle.y - 90, 0);
    }

    private void GetAngle()
    {
        float angle = Vector3.Angle(a3.position - a2.position, a1.position - a2.position);
        angle = Mathf.Clamp(angle, 80, 170);
        Debug.Log(angle);
        float rate = (170 - angle) / 90;
        TempAnim.speed = 1 + rate;
    }
}
