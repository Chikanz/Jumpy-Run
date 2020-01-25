using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns collectable items on platforms
/// </summary>
public class ItemSpawner : PlatformSpawner
{
    public PlatformItem[] Items;
    public float chanceToPlaceItems = 0.25f;

    protected override PlatformObject[] GetObjects()
    {
        return Items;
    }

    protected override void PlatformPlacementLogic(Transform platform)
    {
        if (Random.value < chanceToPlaceItems && !PlatformsPlacedOnThisFrame.Contains(platform))
        {
            var item = Items[Random.Range(0, Items.Length)];
            PlacePlatformObject(item, platform);
        }
    }

    [Serializable]
    public class PlatformItem : PlatformObject
    {
        public int points = 1;
    }
}
