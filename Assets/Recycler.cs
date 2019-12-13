using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds the correct pool of an object it encounters and recycles it
/// </summary>

public class Recycler : MonoBehaviour
{
    public Transform PoolRoot;

    private Dictionary<string, Pool> PoolLookup = new Dictionary<string, Pool>();
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (Pool pool in PoolRoot.GetComponentsInChildren<Pool>())
        {
            PoolLookup.Add(pool.Object.name, pool);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
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
