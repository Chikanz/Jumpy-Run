using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Platforms")]
    public Vector2 heightBetweenObstacles;
    public float zPlaneDistance;
    [FormerlySerializedAs("Obstacles")] public Platform[] platforms;
    private float lastObstacleHeight;

    public Vector2 GapBetweenObsticles;

    private Platform _lastPlacedPlatform;
    private Vector3 lastPlacedObstaclePos;

    public float buildingStartHeight = 40;
    [FormerlySerializedAs("ObstacleHeightClamp")] public Vector2 PlatformHeightClamp;

    public Transform Player; //todo remove into seperate class

    [Header("Pooling")]
    public Transform PoolParent;

    public int TotalPlatformPoolSize;

    private Platform[] ProbabilityPool = new Platform[10];

    public delegate void PlatformPlacedEvent(Transform platform);
    public PlatformPlacedEvent OnPlatformPlaced;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Generate probability Pool List + setup pools
        float chanceTotal = 0;
        int index = 0;
        foreach (Platform platform in platforms)
        {
            //Cache collider width
            platform.Init();
            
            //Object pools
            var poolObj = Instantiate(new GameObject($"{platform.obj.name} Pool"), PoolParent);
            var pool = poolObj.AddComponent<Pool>();
            pool.Object = platform.obj;
            pool.Size = TotalPlatformPoolSize / platforms.Length;
            platform.SetPool(pool);
            pool.Init();
            
            //Probability Pool
            chanceTotal += platform.rarity;
            int loop = Mathf.RoundToInt(platform.rarity * 10);
            for (int i = 0; i < loop; i++)
            {
                ProbabilityPool[index] = platform;
                index++; //Carry over index 
            }
        }
        Debug.Assert(chanceTotal == 1, "Your spawn percentages do not sum to 1!");
        
        //Place first building (long one to start off with)
        var firstPos = new Vector3(0, buildingStartHeight, zPlaneDistance);
        AddNewBuilding(platforms[0], firstPos);
        
        //Place another few platforms to start us off
        for (int i = 0; i < 5; i++)
        {
            PlaceNewBuilding();
        }
        
        //todo remove this
        Player.position = firstPos + Vector3.up * _lastPlacedPlatform.obj.GetComponent<BoxCollider>().size.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 30 == 0) //Check to place buildings every 30 frames
        {
            bool canPlace = true;
            foreach (Platform platform in platforms)
            {
                if (platform.pool.IsEmpty()) canPlace = false;
            }

            if (canPlace)
            {
                PlaceNewBuilding();
            }
        }
    }

    private Platform GetNewObstacle()
    {
        int rand = Random.Range(0, 10);
        return ProbabilityPool[rand];
    }

    //Spawn a building at a position
    private void AddNewBuilding(Platform platform, Vector3 position)
    {
        //Get object from pool
        var platformObj = platform.pool.GetFromPool();

        Transform buildingTransform = platformObj.transform; //cache transform so unity doesn't have to keep getting it from C++
        buildingTransform.rotation = Quaternion.Euler(0, 180, 0);
        buildingTransform.position = position;

        _lastPlacedPlatform = platform;
        lastPlacedObstaclePos = buildingTransform.position;
        
        //Fire out event
        OnPlatformPlaced?.Invoke(platformObj);
    }

    //Places a new building randomly in front of the last
    private void PlaceNewBuilding()
    {
        var obstacle = GetNewObstacle();

        var xTranslate = Vector3.right * (Random.Range((int) GapBetweenObsticles.x, (int) GapBetweenObsticles.y) +
                                          _lastPlacedPlatform.dimensions.x / 2 + obstacle.dimensions.x / 2);
        
        var yTranslate = Vector3.up * Random.Range(heightBetweenObstacles.x, heightBetweenObstacles.y);

        var pos = lastPlacedObstaclePos + xTranslate + yTranslate;

        pos.z = zPlaneDistance; //Lock to our Z distance

        //Clamp Y
        pos.y = Mathf.Clamp(pos.y, PlatformHeightClamp.x, PlatformHeightClamp.y);
        AddNewBuilding(obstacle, pos);
    }
}

[System.Serializable]
public class Platform
{
    public GameObject obj;
    public float rarity;

    public Pool pool { get; private set; }

    public Vector3 dimensions { get; private set; }

    public void Init()
    {
        dimensions = obj.GetComponent<BoxCollider>().size;
    }

    public void SetPool(Pool pool)
    {
        this.pool = pool;
    }
}
