using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballBehaviour : ZGameBehaviour
{

    public const string LeftFootName = "FootLeftMesh";
    public const string RightFootName = "FootRightMesh";

    public GameObject LeftFoot = null;
    public GameObject RightFoot = null;

    public Vector3 FootballPose;
    public GameObject Football;



    public override void Start()
    {
        base.Start();


        if (Initialize) return;

        Initialize = true;

        LeftFoot = GameObject.Find(LeftFootName);
        RightFoot = GameObject.Find(RightFootName);
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

        FootballPose = RightFoot.transform.position + new Vector3(0, 0.05f, -0.35f);

        InitFootballComponent(true);
    }


    public void InitFootballComponent(bool open = true)
    {
        if (open)
        {
            if(Football == null)
            {
                Football = GameObject.Instantiate(Resources.Load<GameObject>("Model/football"));
            }
            Football.transform.position = FootballPose;
        }

        Football.SetActive(open);

        LeftFoot.GetComponent<Collider>().enabled = open;
        RightFoot.GetComponent<Collider>().enabled = open;

      
    }






}
