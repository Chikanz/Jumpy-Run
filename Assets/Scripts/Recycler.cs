using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds the correct pool of an object it encounters and recycles it
/// </summary>

[RequireComponent(typeof(Follow))]
public class Recycler : MonoBehaviour
{
    public Transform PoolRoot;

    private Dictionary<string, Pool> PoolLookup = new Dictionary<string, Pool>();

    private Follow follow;

    private Vector3 startPos;
    
    private void Start()
    {
        startPos = transform.position;
        
        foreach (Pool pool in PoolRoot.GetComponentsInChildren<Pool>())
        {
            try
            {
                PoolLookup.Add(pool.Object.name, pool);
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"{pool.Object.name} has already been pooled. Please make sure there are no " +
                               $"duplicate items being pooled and that all names are unique.");
            }
        }

        follow = GetComponent<Follow>();

        GameManager.Instance.OnStateChanged += OnStateChanged;
        GameManager.Instance.OnResetEarly += () =>
        {
            transform.position = startPos;
        };

    }
    
    private void OnStateChanged(GameManager.eGameState state)
    {
        follow.enabled = state == GameManager.eGameState.RUNNING;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GameState != GameManager.eGameState.RUNNING) return;
        
        Pool pool;
        if (PoolLookup.TryGetValue(other.transform.name, out pool))
        {
            pool.ReturnToPool(other.transform);
        }
        else if (PoolLookup.TryGetValue(other.transform.parent.name, out pool))
        {
            pool.ReturnToPool(other.transform.parent);
        }
        else
        {
            Debug.Log($"Couldn't find pool for {other.name}");
        }
    }
}
