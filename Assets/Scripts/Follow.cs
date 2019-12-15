using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Translate any delta in following object to this object
public class Follow : MonoBehaviour
{
    public Transform Target;

    public float Disance;
    private Vector3 lastPos;

    private bool canFollow = true;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPos = Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFollow)
        {
            transform.position = Target.position + Vector3.left * Disance;
        }
    }
}
