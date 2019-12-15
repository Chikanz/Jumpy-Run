using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Places obstacles on platforms. Also can place items
public class ObstacleSpawner : MonoBehaviour
{
    public Transform PoolRoot;
    public BuildingSpawner spawner;
    public int ObsticlesToPool = 20;

    public Obstacle[] Obstacles;

    private bool firstPlaced = false; //Stop from spawning obsticles on first platform

    private static bool PlacedThisFrame; //Bit of a hack to stop objects spawning in each other

    // Start is called before the first frame update
    void Awake()
    {
        spawner.OnPlatformPlaced += OnPlatformPlaced;

        //Create pools
        foreach (Obstacle obstacle in Obstacles)
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
            firstPlaced = false;
            foreach (Obstacle obstacle in Obstacles)
            {
                obstacle.pool.ForceRecycleAll();
            }
        };
    }

    //Place obstacles on platforms when they're spawned
    private void OnPlatformPlaced(Transform platform)
    {
        if(!firstPlaced)
        {
            firstPlaced = true;
            return;
        }
        
        var index = Random.Range(0, Obstacles.Length);
        var obstacle = Obstacles[index];
        
        //Chance to place
        if (!PlacedThisFrame && Random.value <= obstacle.SpawnChance)
        {
            //Only place on right platforms
            if (obstacle.PlacementMask == string.Empty || platform.name == obstacle.PlacementMask)
            {
                var obsPool = Obstacles[index].pool;
                var obj = obsPool.GetFromPool();

                var pos = platform.position + Vector3.up * platform.GetComponent<BoxCollider>().size.y;
                obj.position = pos;
                obj.rotation = Quaternion.Euler(0, 180, 0);

                PlacedThisFrame = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlacedThisFrame = false;
    }
    
    [Serializable]
    public class Obstacle
    {
        public GameObject Object;
        public string PlacementMask; //If not empty, will only place on this platform
        public float SpawnChance;
        [HideInInspector] public Pool pool;
    }
}
