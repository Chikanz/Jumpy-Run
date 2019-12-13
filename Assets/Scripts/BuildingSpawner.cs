using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    // Start is called before the first frame update
    void Start()
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
            var pool = poolObj.AddComponent<GenericPool>();
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
        AddNewBuilding(platforms[1], firstPos);
        
        //todo remove this
        Player.position = firstPos + Vector3.up * _lastPlacedPlatform.obj.GetComponent<BoxCollider>().size.y;

        for (int i = 0; i < 20; i++)
        {
            var obstacle = GetNewObstacle();
            
            var x = Vector3.right * (Random.Range((int) GapBetweenObsticles.x, (int) GapBetweenObsticles.y) +
                                     _lastPlacedPlatform.width/2 + obstacle.width/2);
            
            Debug.Log(_lastPlacedPlatform.obj.name + " " + x);
            
            var pos = lastPlacedObstaclePos + x + //X
                      Vector3.up * Random.Range(heightBetweenObstacles.x, heightBetweenObstacles.y); //Y

            pos.z = zPlaneDistance; //Lock to our Z distance
            
            //Clamp Y
            pos.y = Mathf.Clamp(pos.y, PlatformHeightClamp.x, PlatformHeightClamp.y);
            AddNewBuilding(obstacle, pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Platform GetNewObstacle()
    {
        int rand = Random.Range(0, 10);
        return ProbabilityPool[rand];
    }

    public void AddNewBuilding(Platform platform, Vector3 position)
    {
        //Get from the right pool
        //var newObj = Instantiate(obstacle.obj, transform);
        var newObj = platform.pool.GetFromPool();
        
        newObj.transform.rotation = Quaternion.Euler(0, 180, 0);
        
        newObj.transform.position = position;

        _lastPlacedPlatform = platform;
        lastPlacedObstaclePos = newObj.transform.position;
    }
    
}

[System.Serializable]
public class Platform
{
    public GameObject obj;
    public float rarity;

    public GenericPool pool { get; private set; }

    public float width { get; private set; }

    public void Init()
    {
        width = obj.GetComponent<BoxCollider>().size.x;
    }

    public void SetPool(GenericPool pool)
    {
        this.pool = pool;
    }
}
