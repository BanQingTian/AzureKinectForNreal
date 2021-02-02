using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareBehaviour : ZGameBehaviour
{
    public const string HandLeftTagName = "LeftHand";
    public const string HandRightTagName = "RightHand";

    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    public GameObject choose;

    public override void ZStart()
    {
        HandLeft = GameObject.FindWithTag(HandLeftTagName);
        HandRight = GameObject.FindWithTag(HandRightTagName);
        if (HandLeft == null | HandRight == null)
        {
            Debug.LogError("Ex!!");
            return;
        }

        if (HandLeft.GetComponent<ZCollision>() == null)
        {
            var zc1 = HandLeft.AddComponent<ZCollision>();
            var zc2 = HandRight.AddComponent<ZCollision>();
            zc1.Init(CollisionTypeEnum.Hand);
            zc2.Init(CollisionTypeEnum.Hand);
        }

        ZDisplay();
    }

    public override void ZDisplay(bool show = true)
    {
        base.ZDisplay(show);

        if (show)
        {
            if (choose == null)
            {
                choose = GameManager.Instance.ChooseMenu; //GameObject.Instantiate(Resources.Load<GameObject>("Model/choose"));
            }
        }

        choose.SetActive(show);

        //HandLeft.GetComponent<Collider>().enabled = show;
        //HandRight.GetComponent<Collider>().enabled = show;

    }

    public override void ZRelease()
    {
        base.ZRelease();

        //GameObject.Destroy(choose.gameObject);
        choose.SetActive(false);
    }

}
