using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZCollision : MonoBehaviour
{
    protected bool ddd = true;

    public void OnTriggerEnter(Collider other)
    {
        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.Football:

                var fb = other.GetComponent<Football>();
                if (fb != null)
                {
                    fb.Rig.isKinematic = false;
                    fb.Rig.AddForce((fb.defaultPos - transform.position) * 1500);
                    StartCoroutine(ResetFootballPos(fb));
                }

                break;
            case GameMode.Drum:

                Debug.Log(other.name);
                var piano = other.GetComponent<PianoKey>();
                if(piano != null && ddd)
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

    private IEnumerator ResetFootballPos(Football fb)
    {
        yield return new WaitForSeconds(2);
        fb.transform.position = fb.defaultPos;
        fb.Rig.isKinematic = true;
    }

}
