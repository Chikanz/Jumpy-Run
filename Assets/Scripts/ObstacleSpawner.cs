using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BuildingSpawner))]

//Places obstacles on platforms. Also can place items
public class ObstacleSpawner : MonoBehaviour
{
    public Transform PoolRoot;
    private BuildingSpawner spawner;
    public int ObsticlesToPool = 20;

    public Obstacle[] Obstacles;
    private List<Pool> Pools = new List<Pool>();
    public float chanceToPlaceObstacle = 0.25f;

    private bool firstPlaced = false; //Stop from spawning obsticles on first platform

    // Start is called before the first frame update
    void Awake()
    {
        spawner = GetComponent<BuildingSpawner>();
        spawner.OnPlatformPlaced += OnPlatformPlaced;

        //Create pools
        foreach (Obstacle obstacle in Obstacles)
        {
            var poolObj = Instantiate(new GameObject($"{obstacle.Object.name} Pool"), PoolRoot);
            var pool = poolObj.AddComponent<Pool>();
            pool.Size = ObsticlesToPool;
            pool.Object = obstacle.Object;
            pool.Init();
            Pools.Add(pool);
        }
    }

    private void OnPlatformPlaced(Transform platform)
    {
        if(!firstPlaced)
        {
            firstPlaced = true;
            return;
        }
        
        //Chance to place
        if (Random.Range(0.0f, 1.0f) <= chanceToPlaceObstacle)
        {
            var index = Random.Range(0, Obstacles.Length);
            var obstacle = Obstacles[index];
            
            //Only place on right platforms
            if (obstacle.PlacementMask == string.Empty || platform.name == obstacle.PlacementMask) 
            {
                var obsPool = Pools[index];
                var obj = obsPool.GetFromPool();

                var pos = platform.position + Vector3.up * platform.GetComponent<BoxCollider>().size.y;
                obj.position = pos;
                obj.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public struct Obstacle
    {
        public GameObject Object;
        public string PlacementMask; //If not empty, will only place on this platform
    }
}
