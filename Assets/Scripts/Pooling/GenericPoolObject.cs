using System;
using UnityEngine;

public class GenericPoolObject : MonoBehaviour, IPoolable
{

    public float TimeCreated { get; set; }

    public void ReturnToPool()
    {
    }

    public void CreatedInPool()
    {
    }
}
