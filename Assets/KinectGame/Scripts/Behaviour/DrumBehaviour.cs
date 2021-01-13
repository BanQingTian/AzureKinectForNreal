using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    public const string HandLeftTagName = "LeftHand";
    public const string HandRightTagName = "RightHand";

    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    public GameObject Drum;
    public GameObject DrumPlusScene;

    public override void ZStart()
    {
        HandLeft = GameObject.FindWithTag(HandLeftTagName);
        HandRight = GameObject.FindWithTag(HandRightTagName);
        if(HandLeft == null | HandRight == null)
        {
            Debug.LogError("Ex!!");
            return;
        }

        if(HandLeft.GetComponent<ZCollision>() == null)
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
                Drum = GameObject.Instantiate(Resources.Load<GameObject>("Model/drum"));
            }
        }

        Drum.SetActive(show);

        HandLeft.GetComponent<Collider>().enabled = show;
        HandRight.GetComponent<Collider>().enabled = show;
    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(Drum.gameObject);
    }

}
