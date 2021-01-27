using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleJump : MonoBehaviour
{
    float defalutY = 0;
    float emulatorG = -9.8f;
    float emulatorY = 3;
    float curUp;
    bool jump = true;
    public void Init()
    {
        defalutY = transform.position.y;
        curUp = emulatorY;
        jump = true;
    }

    private void FixedUpdate()
    {
        if (!jump) return;
        curUp +=  emulatorG * Time.fixedDeltaTime;
        Debug.Log(curUp);
        //transform.position = new Vector3(transform.position.x, transform.position.y + curUp * Time.fixedDeltaTime, transform.position.z);
        transform.position += Vector3.up * curUp * Time.fixedDeltaTime;
        if (transform.position.y < defalutY)
        {
            transform.position = new Vector3(transform.position.x, defalutY, transform.position.z);
            jump = false;
        }
    }
}
