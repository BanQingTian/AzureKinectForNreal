using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGirlController : MonoBehaviour
{
    public GameObject linkedObject;
    public GameObject ghostGirl;

    public float secondsToShow = 6f;
    public float secondsToHide = 2.6f;

    private float counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        ghostGirl.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (linkedObject.activeSelf)
        {
            counter += Time.deltaTime;
        } else
        {
            hide();
        }

        if (counter > secondsToShow && !ghostGirl.activeSelf)
        {
            ghostGirl.SetActive(true);
            Invoke("hide", secondsToHide);
        }
    }

    void hide()
    {
        ghostGirl.SetActive(false);
        counter = 0;
    }
}
