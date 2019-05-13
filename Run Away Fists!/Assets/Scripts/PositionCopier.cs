using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCopier : MonoBehaviour
{
    public MonoBehaviour parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = parent.transform.position;
    }
}
