using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
{
    [Tooltip("The object to spawn, must have ")]
    public GameObject Object;

    private Stack<T> pool = new Stack<T>();
    protected List<T> poolRef = new List<T>(); //For keeping track of all objs, not just inactive ones (not ideal but oh well)

    [Tooltip("How many objects should the pool recycle?")]
    public int Size = 20;

    [Tooltip("How long before an object should return to the pool in seconds? (0 for never)")]
    [SerializeField]
    protected float TimeOut; 
    
    private WaitForSeconds timeOutWait;
    
    public virtual void Init()
    {
        timeOutWait = new WaitForSeconds(TimeOut);       
        
        for (int i = 0; i < Size; i++)
        {
            var obj = Instantiate(Object, transform);
            var script = obj.GetComponent<T>();
            Debug.Assert(script, $"Pooled object must have {nameof(T)} script attached!");
            pool.Push(script);
            poolRef.Add(script);
            script.CreatedInPool();
            obj.gameObject.SetActive(false);
        }
    }

    public virtual T GetFromPool()
    {         
        //Pop stack or get alive the longest
        var obj = pool.Count > 0 ? pool.Pop() : ForceRecycle();        
        obj.gameObject.SetActive(true);
        obj.TimeCreated = Time.time;
        
        InitReturnToPool(obj);
        return obj;
    }

    private T ForceRecycle()
    {
        var t = GetAliveLongest();
        var obj = t.GetComponent<T>();
        ReturnToPool(obj);
        return obj;
    }
    
    private Transform GetAliveLongest()
    {
        int? index = null;
        float minTime = int.MaxValue;
        for (int i = 0; i < poolRef.Count; i++)
        {
            var o = poolRef[i];
            if (o.TimeCreated < minTime)
            {
                index = i;
                minTime = o.TimeCreated;
            }
        }
        
        Debug.Assert(index.HasValue, "All objects are pooled! Why did you call this???");
        return transform.GetChild(index.Value);
    }

    /// <summary>
    /// To setup logic for returning item to pool
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void InitReturnToPool(T obj)
    {
        if (TimeOut > 0) StartCoroutine(TimeOutRoutine(obj));
    }
    
    IEnumerator TimeOutRoutine(T o)
    {
        yield return timeOutWait;
        ReturnToPool(o);
    }

    /// <summary>
    /// Return an item to the pool and disable it
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void ReturnToPool(T obj)
    {
        if (!obj.gameObject.activeSelf) return; //Already deactivted/pooled
        
        obj.ReturnToPool();         
        obj.gameObject.SetActive(false);
        obj.transform.position = Vector3.zero;
        pool.Push(obj);        
    }
}

public interface IPoolable
{
    [HideInInspector]
    float TimeCreated {get; set;}
    
    void ReturnToPool();
    void CreatedInPool();
}


