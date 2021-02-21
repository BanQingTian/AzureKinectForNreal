using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public void Init(Transform parent, Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.SetParent(parent);
        transform.localPosition = pos;
    }
    void FixedUpdate()
    {
        //if (!GameManager.Instance.isCanMove)
        //{
        //    return;
        //}

        //gameObject.transform.localPosition += -Vector3.forward * GameManager.wallMoveaSpeed * 10 * Time.fixedDeltaTime;
        gameObject.transform.localPosition += -Vector3.forward * 10 * Time.fixedDeltaTime;
        if (transform.localPosition.z <= 0)
        {
            PoolManager.Instance.Release(gameObject);
        }
    }
}
