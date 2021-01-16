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
            if (choose == null)
            {
                choose = GameObject.Instantiate(Resources.Load<GameObject>("Model/choose"));
            }
        }

        choose.SetActive(show);

        HandLeft.GetComponent<Collider>().enabled = show;
        HandRight.GetComponent<Collider>().enabled = show;
    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(choose.gameObject);
    }

}
