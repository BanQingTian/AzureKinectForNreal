using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
    public AudioSource AS;
    public Vector3 DefaultPos;


    void Start()
    {
        DefaultPos = transform.position;
    }

    public void Play()
    {
        AS.Play();
        StartCoroutine(keyShake());
    }

    private IEnumerator keyShake()
    {
        float timer = 0;
        while (true)
        {
            if (timer < 0.3f)
            {
                var x = Random.Range(-0.02f, 0.02f);
                var y = Random.Range(-0.02f, 0.02f);
                var z = Random.Range(-0.02f, 0.02f);
                transform.position = new Vector3(x, y, z) + DefaultPos;
            }
            else
            {
                transform.position = DefaultPos;
                yield break;
            }
            timer += Time.deltaTime;
            yield return true;
        }
    }
}
