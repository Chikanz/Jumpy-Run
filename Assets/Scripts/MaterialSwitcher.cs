using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour, IPoolable
{
    public Material[] Materials;

    private MeshRenderer MR;
    
    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponentInChildren<MeshRenderer>();
        
        ReturnToPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ReturnToPool()
    {
        MR.material = Materials[Random.Range(0, Materials.Length)];
    }

    public void CreatedInPool()
    {
    }
}
