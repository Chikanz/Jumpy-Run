using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns obstacles on platforms
/// </summary>
public class ObstacleSpawner : PlatformSpawner
{
    [SerializeField] private Obstacle[] Obstacles;

    protected override PlatformObject[] GetObjects()
    {
        return Obstacles;
    }

    protected override void PlatformPlacementLogic(Transform platform)
    {
        var index = Random.Range(0, Obstacles.Length);
        var obstacle = Obstacles[index];
        
        //Chance to place
        if (Random.value <= obstacle.SpawnChance && !PlacedThisFrame)
        {
            //Only place on right platforms
            if (obstacle.PlacementMask == string.Empty || platform.name == obstacle.PlacementMask) 
            {
                PlacePlatformObject(obstacle, platform);
            }
        }
    }


    [Serializable]
    private class Obstacle : PlatformObject
    {
        public string PlacementMask; //If not empty, will only place on this platform
        public float SpawnChance;
    }
}
