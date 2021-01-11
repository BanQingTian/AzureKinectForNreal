using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballCollision : ZCollision
{
    public Rigidbody rig;

    private Vector3 defaultPose = new Vector3(999, 999, 999);

    private void Start()
    {
        defaultPose = transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Foot"))
        {

            rig.isKinematic = false;
            rig.AddForce((transform.position - other.transform.position) * 1500);
            StartCoroutine(ResetPos());
        }
    }

    private IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(2);
        rig.isKinematic = true;
        transform.position = defaultPose;
    }

    //public void OnTriggerStay(Collider other)
    //{
    //    if (other.name.Contains("Foot"))
    //    {
    //        power+=3;
    //        if (shootDir == Vector3.zero)
    //        {
    //            shootDir = transform.position - other.transform.position;
    //        }
    //    }
    //}

    //public void OnTriggerExit(Collider other)
    //{
    //    if (other.name.Contains("Foot"))
    //    {

    //    }
    //}


    //public void FixedUpdate()
    //{
    //    if (shoot)
    //    {
    //        rig.AddForce(shootDir * power);
    //        power = 0;
    //        shoot = false;
    //        shootDir = Vector3.zero;
    //    }
    //}
}
