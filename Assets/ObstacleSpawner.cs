using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BuildingSpawner))]

public class ObstacleSpawner : MonoBehaviour
{
    public Transform PoolRoot;
    private BuildingSpawner spawner;
    public int ObsticlesToPool = 20;

    public GameObject[] Obstacles;
    private List<Pool> Pools = new List<Pool>();
    public float chanceToPlaceObstacle = 0.25f;

    private bool firstPlaced = false; //Stop from spawning obsticles on first platform

    // Start is called before the first frame update
    void Awake()
    {
        spawner = GetComponent<BuildingSpawner>();
        spawner.OnPlatformPlaced += OnPlatformPlaced;

        //Create pools
        foreach (GameObject obs in Obstacles)
        {
            var poolObj = Instantiate(new GameObject($"{obs.name} Pool"), PoolRoot);
            var pool = poolObj.AddComponent<Pool>();
            pool.Size = ObsticlesToPool;
            pool.Object = obs;
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
        bool isBuilding = platform.name.Equals("Big apartment");
        if (isBuilding && Random.Range(0.0f, 1.0f) <= chanceToPlaceObstacle)
        {
            var obsPool = Pools[Random.Range(0, Obstacles.Length)];
            var obj = obsPool.GetFromPool();
            var pos = platform.position + Vector3.up * platform.GetComponent<BoxCollider>().size.y;
            obj.position = pos;
            obj.rotation = Quaternion.Euler(0,180,0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
