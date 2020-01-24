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
    private Vector3 lastRunnerPosition;

    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI DistanceUI;

    private float Distance;
    private int PickupScore = 0;
    
    public ItemSpawner ItemSpawner;

    
    
    private void Start()
    {
        runnerTransform = runner.transform;

        SetupPickups();

        Reset();

        lastRunnerPosition = runner.transform.position;
        
        GameManager.Instance.OnResetEarly += Reset;
    }
    
    public void Reset()
    {
        Distance = 0;
        PickupScore = 0;
        lastRunnerPosition = runner.transform.position;
    }
    
    /// <summary>
    /// Hook into pickup events so we know when they're picked up
    /// </summary>
    void SetupPickups()
    {
        foreach (PlatformSpawner.PlatformObject obstacle in ItemSpawner.Items)
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
        var runnerDelta = runner.transform.position.x - lastRunnerPosition.x;;
        if (!runner.isDead && runnerDelta > 0) Distance += runnerDelta; 
        lastRunnerPosition = runner.transform.position;

        UpdateUI();
    }

    void UpdateUI()
    {
        ScoreUI.text = PickupScore.ToString();
        DistanceUI.text = $"{(Mathf.Round(Distance/5) * 5).ToString()}m"; //ToString to avoid boxing
    }
}
