using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbController : MonoBehaviour
{
    private Light myLight;

    public float flickerTime = 0.3f;
    public float LightIntensity = 20f;

    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();

        StartCoroutine(Flashing());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            yield return new WaitForSeconds(flickerTime);
            myLight.intensity = flickerTime * LightIntensity;
            myLight.enabled = !myLight.enabled;

        }
    }
}
