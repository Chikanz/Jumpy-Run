using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Obstacles")]
    public Vector2 heightBetweenObstacles;
    public float zPlaneDistance;
    public Obstacle[] Obstacles;
    private float lastObstacleHeight;

    public Vector2 GapBetweenObsticles;

    private Obstacle lastPlacedObstacle;
    private Vector3 lastPlacedObstaclePos;

    public float buildingStartHeight = 40;
    public Vector2 ObstacleHeightClamp;

    public Transform Player; //todo remove into seperate class

    // Start is called before the first frame update
    void Start()
    {
        //cache obstacle width 
        foreach (Obstacle obstacle in Obstacles)
        {
            obstacle.Init();
        }
        
        //Place first building
        var firstPos = new Vector3(0, buildingStartHeight, zPlaneDistance);
        AddNewBuilding(firstPos);

        //Setup player + menu ??
        Player.position = firstPos + Vector3.up * lastPlacedObstacle.obj.GetComponent<BoxCollider>().size.y;

        for (int i = 0; i < 20; i++)
        {
            var pos = lastPlacedObstaclePos + Vector3.right * (Random.Range((int)GapBetweenObsticles.x, (int)GapBetweenObsticles.y) + lastPlacedObstacle.width) + //X
                      Vector3.up * Random.Range(heightBetweenObstacles.x, heightBetweenObstacles.y); //Y

            pos.z = zPlaneDistance; //Lock to our Z distance
            
            //Clamp Y
            pos.y = Mathf.Clamp(pos.y, ObstacleHeightClamp.x, ObstacleHeightClamp.y);
            AddNewBuilding(pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewBuilding(Vector3 position)
    {
        //Get new obstacle
        var obstacle = Obstacles[Random.Range(0, Obstacles.Length)];
        
        //Get from the right pool
        var newObj = Instantiate(obstacle.obj, transform);
        
        newObj.transform.rotation = Quaternion.Euler(0, 180, 0);
        
        newObj.transform.position = position;

        lastPlacedObstacle = obstacle;
        lastPlacedObstaclePos = newObj.transform.position;
    }
    
}

[System.Serializable]
public class Obstacle
{
    public GameObject obj;
    public float rarity;

    public float width { get; private set; }

    public void Init()
    {
        width = obj.GetComponent<BoxCollider>().size.x;
    }
}
