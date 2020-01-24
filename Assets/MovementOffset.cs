using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MovementOffset : MonoBehaviour
{
    public Transform Player;
    private Vector3 PlayerDelta;
    private Vector3 playerLastPos;
    public float ResetDistance = 100;
    public CinemachineVirtualCamera Cam;

    private Transform dummyParent;
    private Transform PlayerCameraPoint;

    private void Start()
    {
        dummyParent = Instantiate(new GameObject("Dummy Parent")).transform;
        PlayerCameraPoint = Player.Find("Cam Follow");
    }

    void FixedUpdate()
    {
        PlayerDelta = Player.position - playerLastPos;
        if (Player.position.x > ResetDistance)
        {
            StartCoroutine(ResetWorldPos());
        }

        playerLastPos = Player.transform.position;
    }

    private IEnumerator ResetWorldPos()
    {
        ToggleCam(false); //Fight cinemachine
        yield return null;
        Cam.enabled = false;
        //Cam.transform.position += PlayerDelta;

        //move back
        transform.position += Vector3.left * ResetDistance;

        //todo test camera rotation delta 
        
        //set parent to dummy
        //SetParent(dummyParent, transform);
            
        //reset position
        //transform.position = Vector3.zero;
            
        //reset parent
        //SetParent(transform, dummyParent);

        Cam.enabled = true;
        yield return null;
        ToggleCam(true);
        Cam.OnTargetObjectWarped(Player, Vector3.left * ResetDistance);

        //yield return null;
        
    }

    void SetParent(Transform To, Transform From)
    {
        foreach (Transform t in From)
        {
            t.SetParent(To);
        }
    }

    void ToggleCam(bool enabled)
    {
        Cam.Follow = enabled ? PlayerCameraPoint : null;
        Cam.LookAt = enabled ? PlayerCameraPoint : null;
    }
}
