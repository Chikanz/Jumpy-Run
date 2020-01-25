using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Attempt to stop floating point errors by moving everything in the world back to 0 after a set distance.
/// </summary>
public class MovementOffset : MonoBehaviour
{
    public Transform Player;
    public CinemachineVirtualCamera Cam;
    [Tooltip("The distance at which every item in the scene is brought back to the world 0")]
    public float ResetDistance = 1000; //Should be around 1,000 - 100,000 according to https://gamedev.stackexchange.com/questions/151732/what-is-actually-moving-in-an-endless-runner 
    private Transform PlayerCameraPoint;
    private Vector3 CamPlayerOffset;

    private void Start()
    {
        PlayerCameraPoint = Player.Find("Cam Follow");
        CamPlayerOffset = Cam.transform.position - PlayerCameraPoint.position;
    }

    void Update()
    {
        if (Player.position.x > ResetDistance && GameManager.Instance.GameState == GameManager.eGameState.RUNNING)
        {
            ResetWorldPos();
        }
    }

    private void ResetWorldPos()
    {
        ToggleCam(false); //Fight cinemachine
        Cam.enabled = false;

        //Move everything back to 0
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position += Vector3.left * ResetDistance;
        }

        //Try to move the camera without cinemachine freaking out
        Cam.enabled = true;
        ToggleCam(true);
        Cam.OnTargetObjectWarped(Player, Vector3.left * ResetDistance);
    }

    void ToggleCam(bool enabled)
    {
        Cam.Follow = enabled ? PlayerCameraPoint : null;
        Cam.LookAt = enabled ? PlayerCameraPoint : null;
    }
}
