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
    private Transform lastPlacedBuildingTransform;

    public float buildingStartHeight = 40;
    [FormerlySerializedAs("ObstacleHeightClamp")] public Vector2 PlatformHeightClamp;

    public Transform Player;

    [Header("Pooling")]
    public Transform PoolParent;

    public int TotalPlatformPoolSize;

    private Platform[] ProbabilityPool = new Platform[10];

    public int InitialPlacement = 3;
    public float PlacementDistanceToPlayer = 50;

    public delegate void PlatformPlacedEvent(Transform platform);
    public PlatformPlacedEvent OnPlatformPlaced;
    
    private void Awake()
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
        
    }

    private void Start()
    {
        StartPlacement();
        GameManager.Instance.OnResetLate += StartPlacement;
        GameManager.Instance.OnResetEarly += RecallAll;    
    }

    private void StartPlacement()
    {
        var firstPos = new Vector3(0, buildingStartHeight, zPlaneDistance);
        AddNewBuilding(platforms[0], firstPos);
        
        //Place another few platforms to start us off
        for (int i = 0; i < InitialPlacement; i++)
        {
            PlaceNewBuilding();
        }
        
        //Place player
        Player.position = firstPos + Vector3.up * _lastPlacedPlatform.obj.GetComponent<BoxCollider>().size.y;
    }

    private void RecallAll()
    {
        foreach (Platform platform in platforms)
        {
            platform.pool.ForceRecycleAll();    
        }
    }
    
    void Update()
    {
        var lastBuildingXDist = lastPlacedBuildingTransform.position.x - Player.position.x;
        if (GameManager.Instance.GameState == GameManager.eGameState.RUNNING && lastBuildingXDist < PlacementDistanceToPlayer) //Check to place buildings every 30 frames
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
    
    //Places a new building randomly in front of the last
    private void PlaceNewBuilding()
    {
        var obstacle = GetNewObstacle();

        var xTranslate = Vector3.right * (Random.Range((int) GapBetweenObsticles.x, (int) GapBetweenObsticles.y) +
                                          _lastPlacedPlatform.dimensions.x / 2 + obstacle.dimensions.x / 2);
        
        var yTranslate = Vector3.up * Random.Range(heightBetweenObstacles.x, heightBetweenObstacles.y);

        var pos = lastPlacedBuildingTransform.position + xTranslate + yTranslate;

        pos.z = zPlaneDistance; //Lock to our Z distance

        //Clamp Y
        pos.y = Mathf.Clamp(pos.y, PlatformHeightClamp.x, PlatformHeightClamp.y);
        AddNewBuilding(obstacle, pos);
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
        lastPlacedBuildingTransform = buildingTransform;
        
        //Fire out event
        OnPlatformPlaced?.Invoke(platformObj);
    }

    [Serializable]
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
}
