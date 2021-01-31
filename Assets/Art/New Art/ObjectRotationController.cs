using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotationController : MonoBehaviour
{
    public float rotationSpeed = 1.6f;
    public bool arroudSelf = false;
    public string axis = "z";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var direction = Vector3.forward;
        if (axis == "x")
        {
            direction = Vector3.right;
        } else if (axis == "y")
        {
            direction = Vector3.up;
        }

        transform.Rotate(direction * Time.deltaTime * rotationSpeed, arroudSelf ? Space.Self : Space.World);
    }
}
