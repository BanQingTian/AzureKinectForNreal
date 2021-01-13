using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZCollision : MonoBehaviour
{
    protected bool ddd = true;
    private Vector3 _lastPos;
    private Vector3 _curPos;

    public void OnCollisionEnter(Collision collision)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Football:

                if (!collision.collider.name.Contains("football")) return;

                float power = (_curPos - _lastPos).magnitude * 4222;
                //Vector3 dir = ((collision.transform.position - collision.contacts[0].point) - (_curPos - _lastPos)).normalized;

                Vector3 dir = (collision.transform.position - collision.contacts[0].point).normalized;

                dir = new Vector3(dir.x * 0.5f, 0.2f, Mathf.Abs(dir.z));

                collision.rigidbody.AddForce(dir * power);

                Debug.Log("a;fja;kdfs;lasfk;ls;fj");
                Debug.DrawRay(collision.contacts[0].point, dir * power * 0.004f, Color.yellow, 6);

                StartCoroutine(ResetFootballPos(collision.transform.GetComponent<Football>()));

                break;
            case GameMode.Drum:
                break;
            default:
                break;
        }
    }


    private void FixedUpdate()
    {
        // update data
        _lastPos = _curPos;
        _curPos = transform.position;
    }


    public void OnTriggerEnter(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Football:

                break;
            case GameMode.Drum:

                Debug.Log(other.name);
                var piano = other.GetComponent<PianoKey>();
                if (piano != null && ddd)
                {
                    piano.Play();
                    ddd = false;
                }

                break;
            default:
                break;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Football:
                break;
            case GameMode.Drum:
                ddd = true;
                break;
            default:
                break;
        }
    }

    bool isDoing = false;
    private IEnumerator ResetFootballPos(Football fb)
    {
        if (isDoing) yield break;
        isDoing = true;
        yield return new WaitForSeconds(6);
        isDoing = false;
        fb.transform.position = fb.defaultPos;
        fb.Rig.isKinematic = true;
        fb.Rig.isKinematic = false;
        fb.Rig.AddForce(new Vector3(1, 1, -0.5f));
    }

}
