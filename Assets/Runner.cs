using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Runner : MonoBehaviour
{
    public float speed;
    public float JumpHeight;

    private Vector3 MoveVector;
    
    private Vector3 JumpVector;
    private Vector3 JumpVectorGoal;

    private float JumpedAtTime;
    public float ApplyJumpTime = 0.25f;

    public float SpeedMulti;
    
    private Rigidbody RB;
    private Animator AC;
    
    
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (SwipeDetector.swipedUp && JumpVectorGoal == Vector3.zero)
        {
            JumpVectorGoal = Vector3.up * JumpHeight;
            JumpedAtTime = Time.time;
        }

        if (Time.time >= JumpedAtTime + ApplyJumpTime)
        {
            JumpVectorGoal = Vector3.zero;
        }

        JumpVector = Vector3.Lerp(JumpVector, JumpVectorGoal, 0.1f);

        speed += SpeedMulti * Time.deltaTime;
        GetComponent<Animator>().speed = speed / 5;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawLine(transform.position, transform.position + JumpVector);
        RB.MovePosition(transform.position + JumpVector + (speed * Time.deltaTime * Vector3.right));
    }
}
