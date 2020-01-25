using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Abstract class for placing things on platforms.
public abstract class PlatformSpawner : MonoBehaviour
{
    public Transform PoolRoot;
    public BuildingSpawner spawner;
    public int ObsticlesToPool = 20;

    private bool firstPlatformPlaced = false; //Stop from spawning obsticles on first platform

    //Contains all the platforms placed this frame. Lets child classes decide if they want to place on a platform with stuff on it
    protected static HashSet<Transform> PlatformsPlacedOnThisFrame = new HashSet<Transform>(); 

    /// <summary>
    /// Get the items to spawn from the child class
    /// </summary>
    /// <returns></returns>
    protected abstract PlatformObject[] GetObjects();

    protected abstract void PlatformPlacementLogic(Transform platform);
    
    private void Awake()
    {
        var pObjects = GetObjects();
        
        spawner.OnPlatformPlaced += OnPlatformPlaced;

        //Create pools
        foreach (PlatformObject obstacle in pObjects)
        {
            var poolObj = Instantiate(new GameObject($"{obstacle.Object.name} Pool"), PoolRoot);
            var pool = poolObj.AddComponent<Pool>();
            pool.Size = ObsticlesToPool;
            pool.Object = obstacle.Object;
            pool.Init();
            obstacle.pool = pool;
        }
        
        //Recall all game objects on reset
        GameManager.Instance.OnResetEarly += () =>
        {
            firstPlatformPlaced = false;
            foreach (PlatformObject pObject in pObjects)
            {
                pObject.pool.ForceRecycleAll();
            }
        };
    }

    /// <summary>
    /// Called when a new platform is spawned. Override to implement placement behaviour. Make sure to call base first!
    /// </summary>
    /// <param name="platform">Transform of the platform that just spawned</param>
    private void OnPlatformPlaced(Transform platform)
    {
        if(!firstPlatformPlaced) //Don't place on first platform
        {
            firstPlatformPlaced = true;
            return;
        }
        
        PlatformPlacementLogic(platform);
    }

    /// <summary>
    /// Places an object on a platform
    /// </summary>
    /// <param name="platformObject">The object to place</param>
    /// <param name="platform">The platform to place it on</param>
    protected virtual void PlacePlatformObject(PlatformObject platformObject, Transform platform)
    {
        var obsPool = platformObject.pool;
        var obj = obsPool.GetFromPool();

        var pos = platform.position + Vector3.up * platform.GetComponent<BoxCollider>().size.y;
        obj.position = pos;
        obj.rotation = Quaternion.Euler(0, 180, 0);
        
        PlatformsPlacedOnThisFrame.Add(platform);
    }
    
    
    protected virtual void Update()
    {
        PlatformsPlacedOnThisFrame.Clear();
    }
    
    [Serializable]
    public class PlatformObject
    {
        public GameObject Object;
        [HideInInspector] public Pool pool;
    }
}
