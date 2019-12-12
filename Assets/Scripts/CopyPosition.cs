using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform target;

    public bool X;
    public bool Y;
    public bool Z;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!X && !Y && !Z) return;
        
        transform.position = new Vector3(
            X ? target.position.x : transform.position.x,
            Y ? target.position.y : transform.position.y,
            Z ? target.position.z : transform.position.z
            );
    }
}
