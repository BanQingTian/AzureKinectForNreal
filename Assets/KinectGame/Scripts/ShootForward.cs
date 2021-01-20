using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootForward : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2);
        transform.rotation = Quaternion.identity;
        transform.parent = null;
    }

    float cur = 1.4f;
    float g = -9.8f;
    float time = 0;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            cur += g ;
            time = 0;
        }
        cur += g * Time.deltaTime;
        transform.position += new Vector3(0, cur * Time.fixedDeltaTime, 20 * Time.deltaTime);
    }
}
