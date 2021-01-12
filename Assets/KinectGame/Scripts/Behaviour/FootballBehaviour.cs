using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballBehaviour : ZGameBehaviour
{

    public const string LeftFootName = "FootLeftMesh";
    public const string RightFootName = "FootRightMesh";

    public const string LeftFootRootName = "FootLeft";
    public const string RightFootRootName = "FootRight";

    public const string FootballFieldName = "FootballField";



    public GameObject LeftFoot = null;
    public GameObject RightFoot = null;
    public GameObject LeftFootRoot = null;
    public GameObject RightFootRoot = null;

    public Vector3 FootballPose;
    public GameObject Football;
    public GameObject FootballField;



    public override void ZStart()
    {
        base.ZStart();

        if (LeftFoot == null)
        {
            LeftFoot = GameObject.Instantiate(Resources.Load<GameObject>("model/FootLeftMesh"));
            RightFoot = GameObject.Instantiate(Resources.Load<GameObject>("model/FootRightMesh"));

            LeftFootRoot = GameObject.Find(LeftFootRootName);
            RightFootRoot = GameObject.Find(RightFootRootName);
            FootballField = GameObject.Instantiate(Resources.Load<GameObject>("model/FootballField"));
        }

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

        //FootballPose = RightFoot.transform.position + new Vector3(0, 0.05f, -0.35f);

        ZDisplay(true);
    }

    public override void ZUpdate()
    {
        base.ZUpdate();

        LeftFoot.transform.position = LeftFootRoot.transform.position;
        RightFoot.transform.position = RightFootRoot.transform.position;
    }

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

        Football.SetActive(show);
        Football.GetComponent<Rigidbody>().AddForce(new Vector3(1,1,-0.5f));
        FootballField.SetActive(show);
        LeftFoot.GetComponent<Collider>().enabled = show;
        RightFoot.GetComponent<Collider>().enabled = show;

    }

    public override void ZRelease()
    {
        base.ZRelease();

        GameObject.Destroy(Football.gameObject);
    }

}
