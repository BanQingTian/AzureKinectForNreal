using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBehaviour : ZGameBehaviour
{
    public const string HandLeftName = "HandLeft";
    public const string HandRightName = "HandRight";

    public GameObject HandLeft = null;
    public GameObject HandRight = null;

    public GameObject Drum;

    public override void Start()
    {
        HandLeft = GameObject.Find(HandLeftName);
        HandRight = GameObject.Find(HandRightName);
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

        InitFootballComponent(true);
    }

    public override void Release()
    {
        base.Release();

        GameObject.Destroy(Drum.gameObject);
    }


    public void InitFootballComponent(bool open = true)
    {
        if (open)
        {
            if (Drum == null)
            {
                Drum = GameObject.Instantiate(Resources.Load<GameObject>("Model/drum"));
            }
        }

        Drum.SetActive(open);

        HandLeft.GetComponent<Collider>().enabled = open;
        HandRight.GetComponent<Collider>().enabled = open;


    }
}
