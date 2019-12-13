using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [Tooltip("The object to spawn, must have ")]
    public GameObject Object;

    private Stack<Transform> pool = new Stack<Transform>();
    protected List<Transform> poolRef = new List<Transform>(); //For keeping track of all objs, not just inactive ones (not ideal but oh well)

    [Tooltip("How many objects should the pool recycle?")]
    public int Size = 20;
    
    
    public virtual void Init()
    {

        for (int i = 0; i < Size; i++)
        {
            var obj = Instantiate(Object, transform);
            obj.name = Object.name;
            pool.Push(obj.transform);
            TryGetPoolable(obj.transform)?.CreatedInPool();
            obj.gameObject.SetActive(false);
        }
    }

    public virtual Transform GetFromPool()
    {         
        //Pop stack or get alive the longest
        Debug.Assert(pool.Count > 0, "No more items to pool!");
        var obj = pool.Pop();
        obj.gameObject.SetActive(true);
        
        return obj;
    }
    

    /// <summary>
    /// Return an item to the pool and disable it
    /// </summary>
    /// <param name="obj"></param>
    public virtual void ReturnToPool(Transform obj)
    {
        if (!obj.gameObject.activeSelf) return; //Already deactivted/pooled

        //If component has a poolable interface tell it that it's been returned to pool
        TryGetPoolable(obj)?.ReturnToPool();

        obj.gameObject.SetActive(false);
        obj.transform.position = Vector3.zero;
        pool.Push(obj);        
    }

    private IPoolable TryGetPoolable(Transform obj)
    {
        var component = obj.GetComponent(typeof(IPoolable));
        if (component)
        {
            var poolable = component as IPoolable;
            if (poolable != null) return poolable;
        }

        return null;
    }

    public bool IsEmpty()
    {
        return pool.Count == 0;
    }
}


public interface IPoolable
{
    void ReturnToPool();
    void CreatedInPool();
}


