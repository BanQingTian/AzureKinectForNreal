using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballBehaviour : ZGameBehaviour
{

    public const string LeftFootTag = "LeftFoot";
    public const string RightFootTag = "RightFoot";

    public const string FootballFieldName = "FootballField";


    public GameObject LeftFoot = null;
    public GameObject RightFoot = null;

    public Vector3 FootballPose;
    public GameObject Football;
    public GameObject FootballField;



    public override void ZStart()
    {
        base.ZStart();

        LeftFoot = GameObject.FindWithTag(LeftFootTag);
        RightFoot = GameObject.FindWithTag(RightFootTag);
        FootballField = GameObject.Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}", Dir, FootballFieldName)));

        if (LeftFoot == null | RightFoot == null)
        {
            Debug.LogError("Ex!!");
            return;
        }

        if (!LeftFoot.GetComponent<ZCollision>())
        {
            LeftFoot.AddComponent<ZCollision>();
            RightFoot.AddComponent<ZCollision>();
        }

        ZDisplay(true);
    }

    //public override void ZUpdate()
    //{
    //    base.ZUpdate();

    //    LeftFoot.transform.position = LeftFootRoot.transform.position;
    //    RightFoot.transform.position = RightFootRoot.transform.position;
    //}

    public override void ZDisplay(bool show = true)
    {
        base.ZDisplay(show);

        if (show)
        {
            if (Football == null)
            {
                Football = GameObject.Instantiate(Resources.Load<GameObject>("Model/football"));
            }
        }

        Football.transform.position = Football.GetComponent<Football>().defaultPos;
        Football.SetActive(show);
        Football.GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, -0.5f));
        FootballField.SetActive(show);

        LeftFoot.GetComponent<Collider>().enabled = show;
        RightFoot.GetComponent<Collider>().enabled = show;

    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(Football.gameObject);
        GameObject.Destroy(FootballField.gameObject);
    }

}
