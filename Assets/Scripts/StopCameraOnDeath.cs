using System;
using Cinemachine;
using UnityEngine;

public class StopCameraOnDeath : MonoBehaviour
{
    public Runner runner;

    private CinemachineVirtualCamera CVC;

    private ScreenShaker SS;

    // Start is called before the first frame update
    void Start()
    {
        CVC = GetComponent<CinemachineVirtualCamera>();
        SS = GetComponent<ScreenShaker>();
        runner.OnDeath += OnDeath;
    }

    private void OnDeath(object sender, EventArgs e)
    {
        CVC.Follow = null;
        CVC.LookAt = null;
        
        SS.Shake();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
}
