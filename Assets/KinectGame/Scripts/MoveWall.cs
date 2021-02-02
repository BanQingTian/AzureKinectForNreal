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
        gameObject.transform.localPosition += -Vector3.forward * GameManager.wallMoveaSpeed * 3 * Time.fixedDeltaTime;
        if (transform.localPosition.z <= 0)
        {
            PoolManager.Instance.Release(gameObject);
        }
    }
}
