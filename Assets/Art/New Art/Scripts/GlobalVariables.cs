using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class GlobalVariables : MonoBehaviour
{
    public NRLaserVisual theLaser;
    //public GameObject theLight;
    public AudioSource sound_appHover;
    public AudioSource sound_clicked;

    private static GlobalVariables instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static GlobalVariables GetInstance()
    {
        return instance;
    }

    //

    public NRLaserVisual getLaserVisual()
    {
        return theLaser;
    }

    //public GameObject getLight()
    //{
    //    return theLight;
    //}

    public AudioSource getSound_appHover()
    {
        return sound_appHover;
    }

    public AudioSource getSound_clicked()
    {
        return sound_clicked;
    }
}
