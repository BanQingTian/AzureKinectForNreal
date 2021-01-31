using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class TestModeController : MonoBehaviour
{
    public List<GameObject> gameObjects;

    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            index += 1;
            if (index >= gameObjects.Count)
            {
                index = 0;
            }
        }

        for(int i = 0; i < gameObjects.Count; i++)
        {
            if (i == index)
            {
                gameObjects[i].SetActive(true);
            } else
            {
                gameObjects[i].SetActive(false);
            }
        }
    }
}
