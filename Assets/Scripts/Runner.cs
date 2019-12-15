using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Runner : MonoBehaviour
{
    [Header("Locomotion")] 
    public float startSpeed;
    private float speed;
    [Tooltip("Controls how big the jump vector is")]
    public float JumpHeight;
    [Tooltip("How long the jump vector is applied for")]
    public float ApplyJumpTime = 0.25f;
    [Tooltip("How fast to increase the running speed")]
    public float SpeedMulti;
    [Tooltip("Fall height from which the landing is a roll")]
    public float RollHeight = 2;
    [Tooltip("Clamps speed at this amount")]
    public float MaxSpeed = 14;
    [Tooltip("How much to slow down when sliding")]
    float slideSlowDown = 0.5f;
    [Tooltip("How long the player is in the air before a fall is triggered")]
    public float triggerFallAfter = 0.5f;
    
    [Header("Objects")]
    public CapsuleCollider RunCollider;
    public CapsuleCollider SlideCollider;

    private Vector3 MoveVector;
    private Vector3 JumpVector;
    private Vector3 JumpVectorGoal;
    
    private float JumpedAtTime;
    
    private Rigidbody RB;
    private Animator AC;

    private bool inAir;
    private float inAirTimer;

    private bool falling = false;
    private float FellFromY;

    private Rigidbody[] Ridgidbodies;
    private Collider[] RagdollColliders;
    
    public bool isDead { get; private set; }

    private bool sliding;

    public EventHandler OnDeath;
    
    #region AnimatorHashes
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int FallingHash = Animator.StringToHash("Falling");
    private static readonly int LandedHash = Animator.StringToHash("Landed");
    private static readonly int LandingTypeHash = Animator.StringToHash("LandingType");
    private static readonly int SlideHash = Animator.StringToHash("Slide");
    private static readonly int InAirHash = Animator.StringToHash("InAir");
    private static readonly int ResetHash = Animator.StringToHash("Reset");

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        speed = startSpeed;
        
        RB = GetComponent<Rigidbody>();
        AC = GetComponent<Animator>();

        Ridgidbodies = GetComponentsInChildren<Rigidbody>();
        RagdollColliders = GetComponentsInChildren<Collider>();
        ToggleRagdoll(false);
        
        RB.isKinematic = false;

        GameManager.Instance.OnResetEarly += Reset;
    }
    
    private void Update()
    {
        if(isDead) return;

        if (SwipeDetector.swipedDown && !inAir && !sliding)
        {
            StartCoroutine(Slide());
        }

        if (SwipeDetector.swipedUp && !inAir)
        {
            JumpVectorGoal = Vector3.up * JumpHeight;
            JumpedAtTime = Time.time;
            AC.SetTrigger(JumpHash);
            inAir = true;
            FellFromY = transform.position.y;
        }

        if (Time.time >= JumpedAtTime + ApplyJumpTime)
        {
            JumpVectorGoal = Vector3.zero;
        }

        JumpVector = Vector3.Lerp(JumpVector, JumpVectorGoal, 0.1f);

        if(speed < MaxSpeed) speed += SpeedMulti * Time.deltaTime;

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
        ToggleSlide(sliding || AC.GetCurrentAnimatorStateInfo(0).IsName("Falling To Roll")); 

        AC.SetBool(InAirHash, inAir);
    }

    private void ToggleSlide(bool enabled)
    {
        RunCollider.enabled = !enabled;
        SlideCollider.enabled = enabled;
    }

    private IEnumerator Slide()
    {
        sliding = true;
        AC.SetTrigger(SlideHash);
        yield return new WaitForSeconds(0.25f);
        speed -= slideSlowDown;
        yield return new WaitForSeconds(0.5f);
        sliding = false;
    }
    
    private void Die()
    {
        if(isDead) return; //Can't die twice

        isDead = true;
        ToggleRagdoll(true);
        AC.enabled = false;

        OnDeath?.Invoke(this, null); //Fire off death event
        
        GameManager.Instance.ChangeState(GameManager.eGameState.GAMEOVER);
    }

    void Reset()
    {
        speed = startSpeed;
        isDead = false;
        ToggleRagdoll(false);
        transform.rotation = Quaternion.Euler(0, 90, 0);
        AC.enabled = true;
        AC.SetTrigger(ResetHash);
        RB.isKinematic = false;
        inAir = false;
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

        var dot = Vector3.Dot(other.contacts[0].normal, Vector3.left);
        
        if (dot > 0.85f)
        {
            Die();
        }
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
        RB.constraints = enabled ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotation;

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
