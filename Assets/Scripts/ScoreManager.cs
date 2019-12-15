using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles Scoring and displaying score in the UI
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public Runner runner;
    private Transform runnerTransform;

    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI DistanceUI;

    private int Distance;
    private int PickupScore = 0;
    
    public ObstacleSpawner ItemSpawner;

    private Vector3 StartPosition;


    // Start is called before the first frame update
    void Start()
    {
        runnerTransform = runner.transform;

        SetupPickups();

        Reset();

        StartPosition = runner.transform.position;
        
        GameManager.Instance.OnResetEarly += Reset;
    }
    
    public void Reset()
    {
        Distance = 0;
        PickupScore = 0;
    }


    /// <summary>
    /// Hook into pickup events so we know when they're picked up
    /// </summary>
    void SetupPickups()
    {
        foreach (ObstacleSpawner.Obstacle obstacle in ItemSpawner.Obstacles)
        {
            var items = obstacle.pool.Items();
            foreach (Transform itemTransform in items)
            {
                itemTransform.GetComponent<ItemPickup>().OnPickup += OnItemPickup;
            }
        }
    }


    private void OnItemPickup(object sender, EventArgs e)
    {
        PickupScore++;
    }

    // Update is called once per frame
    void Update()
    {
        //Compare X change instead of vector3.distance to avoid square root calcs
        if(!runner.isDead) Distance = Mathf.RoundToInt(runnerTransform.position.x - StartPosition.x);

        UpdateUI();
    }

    void UpdateUI()
    {
        ScoreUI.text = PickupScore.ToString();
        DistanceUI.text = $"{(Mathf.Round(Distance/5) * 5).ToString()}m"; //ToString to avoid boxing
    }
}
