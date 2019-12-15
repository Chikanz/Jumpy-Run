using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Translate any delta in following object to this object
public class Follow : MonoBehaviour
{
    public Transform follow;
    private Vector3 lastPos;

    private bool canFollow = true;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPos = follow.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFollow)
        {
            var followPosition = follow.position;

            var delta = followPosition - lastPos;
            transform.position += delta;
            lastPos = followPosition;
        }
    }
}
