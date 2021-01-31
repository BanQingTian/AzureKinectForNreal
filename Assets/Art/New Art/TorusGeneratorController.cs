using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusGeneratorController : MonoBehaviour
{

    public GameObject objectPrefab;
    public float objectRadius = 0.05f;
    private float objectSpeed = 0.5f;
    public Vector3 refCenter = Vector3.zero;
    public float radius = 0.2f;
    public float spaceDistance = 1.8f;
    private float atDistance = 1.8f;

    private GameObject lastObject;
    private int count = 0;

    private int deca = 2;
    private bool extraRest = true;


    // Use this for initialization
    void Start()
    {
        points = generateSimplePoints();
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("count: " + count);

        float at = atDistance - spaceDistance;
        //if (!pathFeed)
        //{
        //    at = Random.Range(atDistance - 1.0f, atDistance - 0.35f);
        //    if (extraRest) at = 0.2f;
        //}

        if (count == 0)
        {
            addObject();
        }
        else if (lastObject.transform.localPosition.z < at)
        {
            addObject();
        }

    }

    private void addObject()
    {
        GameObject objectInstance = Instantiate(objectPrefab);
        objectInstance.transform.SetParent(transform);

        float x = 0; // Random.Range(-(radius - objectRadius), (radius - objectRadius));
        float y = 0; // Random.Range(-(radius - objectRadius), (radius - objectRadius));
        objectInstance.transform.rotation = Quaternion.identity;
        objectInstance.transform.localPosition = new Vector3(x, y, atDistance);
        //
        extraRest = false;


        lastObject = objectInstance;
        count += 1;
    }

    public void objectRemoved()
    {
        count -= 1;
    }

    private Vector3[] points;
    private int many = 20;
    private int manyCounter = 0;
    Vector3[] generateSimplePoints()
    {
        float x = 0;
        float y = 0;

        float decaZ = 0.05f;

        Vector3[] result = new Vector3[many];

        float angle = Random.Range(0, 360);
        x = -Mathf.Cos(angle) * 0.2f;
        y = -Mathf.Sin(angle) * 0.2f;

        for (int i = 0; i < many; i++)
        {
            result[i] = new Vector3(x, y, decaZ);
            float r = Mathf.Sin(Mathf.PI * 2 * (float)i / 20) / 80;
            x += Mathf.Cos(angle) * r;
            y += Mathf.Sin(angle) * r;
        }

        return result;

    }

}
