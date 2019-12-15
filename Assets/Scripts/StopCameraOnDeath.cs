using System;
using Cinemachine;
using UnityEngine;

public class StopCameraOnDeath : MonoBehaviour
{
    public Runner runner;
    private Transform camFollow;

    private CinemachineVirtualCamera CVC;

    private ScreenShaker SS;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        CVC = GetComponent<CinemachineVirtualCamera>();
        SS = GetComponent<ScreenShaker>();
        GameManager.Instance.OnStateChanged += StateChanged;
        
        camFollow = runner.transform.Find("Cam Follow");
    }

    private void StateChanged(GameManager.eGameState state)
    {
        switch (state)
        {
            case GameManager.eGameState.RUNNING:
                
                //transform.position = startPos;
                
                CVC.Follow = camFollow;
                CVC.LookAt = camFollow;
                break;
            
            case GameManager.eGameState.GAMEOVER:
                CVC.Follow = null;
                CVC.LookAt = null;
        
                SS.Shake();
                break;
        }
    }

    private void OnDeath(object sender, EventArgs e)
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
}
