using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles pooling and game interaction of items in the world 
/// </summary>
public class ItemPickup : MonoBehaviour, IPoolable
{
    private bool pickedUp = false;
    private MeshRenderer MR;
    private ParticleSystem PS;
    public EventHandler OnPickup;
    
    void Awake()
    {
        MR = GetComponentInChildren<MeshRenderer>();
        PS = GetComponentInChildren<ParticleSystem>();
    }

    public void ReturnToPool()
    {
        TogglePickedUp(false);
    }

    public void CreatedInPool()
    {
        
    }

    private void TogglePickedUp(bool enabled)
    {
        pickedUp = enabled;
        MR.enabled = !enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TogglePickedUp(true);
            PS.Play();
            OnPickup?.Invoke(this, null);
        }
    }
}
