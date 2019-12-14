using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPoolable
{
    private bool pickedUp = false;
    private MeshRenderer MR;
    private ParticleSystem PS;
    public EventHandler OnPickup;
    
    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponent<MeshRenderer>();
        PS = transform.parent.GetChild(1).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToPool()
    {
        
    }

    public void CreatedInPool()
    {
        TogglePickedUp(false);
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
