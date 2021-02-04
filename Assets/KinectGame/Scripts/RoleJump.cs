using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleJump : MonoBehaviour
{
    float defalutY = 0;
    float emulatorG = -9.8f;
    float emulatorY = 3f;
    float upward;
    bool jump = true;
    public void Init(float y)
    {
        defalutY =y;
        upward = emulatorY;
        jump = true;
    }

    private void FixedUpdate()
    {
        if (!jump) return;
        upward +=  emulatorG * Time.fixedDeltaTime;
        transform.position += Vector3.up * upward * Time.fixedDeltaTime;
        if (transform.position.y < defalutY)
        {
            transform.position = new Vector3(transform.position.x, defalutY, transform.position.z);
            jump = false;
        }
    }
}
