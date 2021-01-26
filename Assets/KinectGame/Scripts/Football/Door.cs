using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public MeshRenderer MR;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("football"))
        {
            PlayEff();
        }
    }

    private void PlayEff()
    {
        float r = Random.Range(0, 1f);
        float g = Random.Range(0, 1f);
        float b = Random.Range(0, 1f);

        MR.material.color = new Color(r, g, b);
    }
}
