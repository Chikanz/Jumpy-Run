using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Runner runner;
    
    //todo place player on first building
    //todo hookup UI
    //todo resetti the spaghetti 
    
    public int Pickups { get; private set; } = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //Hook into item pickups
        var items = FindObjectsOfType<ItemPickup>();
        foreach (ItemPickup item in items)
        {
            item.OnPickup += OnItemPickup;
        }
    }

    private void OnItemPickup(object sender, EventArgs e)
    {
        Pickups++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
