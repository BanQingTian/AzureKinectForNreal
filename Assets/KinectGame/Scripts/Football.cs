﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Football : MonoBehaviour
{
    public Rigidbody Rig;
    public Vector3 defaultPos;
    void Start()
    {
        defaultPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
