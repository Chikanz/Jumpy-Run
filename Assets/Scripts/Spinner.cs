using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 direction;
    public bool world;
    public float speed;
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(direction, speed * Time.deltaTime, world ? Space.World : Space.Self);
    }
}
