using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusController : MonoBehaviour
{
    private float speed = 1.6f;
    public float rotationSpeed = 1.6f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(-Vector3.forward * Time.deltaTime * speed);
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed, Space.Self);

        if (transform.position.z <= 0)
        {
            remove();
        }

    }

    private void remove()
    {
        if (gameObject != null)
        {
            //Destroy(gameObject);
        }
    }
}
