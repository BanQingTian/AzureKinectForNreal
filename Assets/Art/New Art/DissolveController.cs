using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class DissolveController : MonoBehaviour
{
    public GameObject model;
    public GameObject alias;
    public GameObject plane;
    public ParticleSystem particle;

    private bool start = false;
    private Vector3 startPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        alias.SetActive(false);

        startPos = plane.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            particle.Play();

            Invoke("startDissolve", 1.5f);

            Invoke("resetAll", 4f);
        }

        if (start)
        {
            plane.transform.localPosition = Vector3.Lerp(plane.transform.localPosition, startPos + new Vector3(0, 3f, 0), 2 * Time.deltaTime);
        }
    }

    void startDissolve()
    {
        model.SetActive(false);
        alias.SetActive(true);

        start = !start;

        model.GetComponent<SkinnedMeshRenderer>().BakeMesh(alias.GetComponent<MeshFilter>().mesh);
    }

    void resetAll()
    {
        start = false;
        plane.transform.localPosition = startPos;
        alias.SetActive(false);
        model.SetActive(true);

        MeshRenderer mrPlane = plane.GetComponent<MeshRenderer>();
        mrPlane.material.SetFloat("_Alpha", 0f);
    }
}
