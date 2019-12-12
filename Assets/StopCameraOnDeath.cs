using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StopCameraOnDeath : MonoBehaviour
{
    public Runner runner;

    private CinemachineVirtualCamera CVC;
    
    // Start is called before the first frame update
    void Start()
    {
        CVC = GetComponent<CinemachineVirtualCamera>();
        runner.OnDeath += OnDeath;
    }

    private void OnDeath(object sender, EventArgs e)
    {
        CVC.Follow = null;
        CVC.LookAt = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
