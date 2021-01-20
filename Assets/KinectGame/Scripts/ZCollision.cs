using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZCollision : MonoBehaviour
{
    protected bool ddd = true;
    private Vector3 _lastPos;
    private Vector3 _curPos;

    public GameObject DelayGO;

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

        changeTime += Time.fixedDeltaTime;

        if (cubeFollow)
        {
            if (/*Vector3.Distance(curFollower.transform.position, DelayGO.transform.position) > 0.2f && */curFollower.transform.position.z-DelayGO.transform.position.z>0.15f)
            {
                cubeFollow = false;
                curFollower.AddComponent<ShootForward>();
                curFollower = null;

            }
        }
        if (follow)
        {
            delayFollow();
        }
    }

    float changeTime = 2;
    public void OnTriggerEnter(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Prepare:

                if (other.name == "C1")
                {
                    if (changeTime < 2f)
                        return;
                    changeTime = 0;
                    GameManager.Instance.ChangePlayerRole(GameManager.Instance.CurPlayerRoleModel + 1);
                }
                else
                {
                    if (other.name == "C2")
                    {
                        GameManager.Instance.ChangeGameMode();
                    }
                }

                break;

            case GameMode.Football:

                break;
            case GameMode.Drum:

                var piano = other.GetComponent<PianoKey>();
                if (piano != null && ddd)
                {
                    piano.Play();
                    ddd = false;
                    if (!cubeFollow)
                    {
                        cubeFollow = true;
                        piano.transform.SetParent(transform);
                        piano.transform.localPosition = Vector3.zero;
                        piano.GetComponent<Collider>().enabled = false;
                        ResetDelayFollow();
                        curFollower = piano.gameObject;
                    }
                }

                break;
            default:
                break;
        }
    }

    bool cubeFollow = false;
    GameObject curFollower = null;

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

    bool follow = false;
    private void ResetDelayFollow()
    {
        if (DelayGO == null)
        {
            DelayGO = new GameObject("delay_go");
        }
        DelayGO.transform.position = transform.position;
        follow = true;
    }
    private void delayFollow()
    {
        DelayGO.transform.position = Vector3.MoveTowards(DelayGO.transform.position, transform.position, 0.2f*Time.deltaTime);
    }
}
