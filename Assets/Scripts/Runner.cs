﻿using System;
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

    [Tooltip("How long to the player is in the air before a fall is triggered")]
    public float triggerFallAfter = 0.5f;
    
    private float JumpedAtTime;
    public float ApplyJumpTime = 0.25f;

    public float SpeedMulti;
    
    private Rigidbody RB;
    public CapsuleCollider RunCollider;
    public CapsuleCollider SlideCollider;
    private Animator AC;
    private bool inAir;
    private float inAirTimer;
    
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int FallingHash = Animator.StringToHash("Falling");
    private static readonly int LandedHash = Animator.StringToHash("Landed");
    private static readonly int LandingTypeHash = Animator.StringToHash("LandingType");
    private static readonly int SlideHash = Animator.StringToHash("Slide");
    private static readonly int InAirHash = Animator.StringToHash("InAir");

    private bool falling = false;
    private float FellFromY;
    public float RollHeight = 2;

    private Rigidbody[] Ridgidbodies;
    private Collider[] RagdollColliders;
    

    private bool isDead;

    public EventHandler OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        AC = GetComponent<Animator>();

        Ridgidbodies = GetComponentsInChildren<Rigidbody>();
        RagdollColliders = GetComponentsInChildren<Collider>();
        ToggleRagdoll(false);
        
        RB.isKinematic = false;
    }

    private void Update()
    {
        if(isDead) return;

        if (SwipeDetector.swipedDown && !inAir)
        {
            AC.SetTrigger(SlideHash);
        }

        if (SwipeDetector.swipedUp && !inAir)
        {
            JumpVectorGoal = Vector3.up * JumpHeight;
            JumpedAtTime = Time.time;
            AC.SetTrigger(JumpHash);
            inAir = true;
        }

        if (Time.time >= JumpedAtTime + ApplyJumpTime)
        {
            JumpVectorGoal = Vector3.zero;
        }

        JumpVector = Vector3.Lerp(JumpVector, JumpVectorGoal, 0.1f);

        speed += SpeedMulti * Time.deltaTime;

        //In air timer for rolling + anmimations
        if (inAir)
        {
            inAirTimer += Time.deltaTime;

            //Fall after a set time after jump, or fall after running off a building without jumping 
            if ((inAirTimer > triggerFallAfter || JumpVectorGoal == Vector3.zero) && !falling) 
            {
                AC.SetTrigger(FallingHash);
                falling = true;
                FellFromY = transform.position.y;
            }
        }
        
        //Slide collider
        ToggleSlide(AC.GetCurrentAnimatorStateInfo(0).IsName("Slide"));

        AC.SetBool(InAirHash, inAir);
    }

    private void ToggleSlide(bool enabled)
    {
        RunCollider.enabled = !enabled;
        SlideCollider.enabled = enabled;
    }
    

    private void OnCollisionEnter(Collision other)
    {
        if(isDead) return; //Don't bother if dead

        if (inAir && other.gameObject.CompareTag("Building"))
        {
            inAir = false;
            falling = false;
            inAirTimer = 0;
            AC.SetTrigger(LandedHash);
            
            var landingType = FellFromY - transform.position.y > RollHeight ? 1 : 0; //set landing type based on fall height
            AC.SetInteger(LandingTypeHash, landingType);
        }

        var dot =Vector3.Dot(other.contacts[0].normal, Vector3.up);
        
        if (dot < 0.8f)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if(isDead) return; //Can't die twice

        isDead = true;
        ToggleRagdoll(true);
        AC.enabled = false;

        OnDeath?.Invoke(this, null); //Fire off death event
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Building"))
        {
            inAir = true;
            FellFromY = transform.position.y;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (inAir && other.gameObject.CompareTag("Building"))
        {
            inAir = false;
            falling = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RB.MovePosition(transform.position + JumpVector + (speed * Time.deltaTime * Vector3.right));
    }

    void ToggleRagdoll(bool enabled)
    {
        if (enabled) RB.constraints = RigidbodyConstraints.None;
        
        foreach (Rigidbody RB in Ridgidbodies)
        {
            RB.isKinematic = !enabled;
        }

        foreach (Collider Colliders in RagdollColliders)
        {
            Colliders.enabled = enabled;
        }
    }
}
