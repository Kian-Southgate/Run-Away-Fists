using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManualController : MonoBehaviour
{
    public float speed;             //Floating point variable to store the player's movement speed.
    
    CharacterController controller;
     

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        Vector3 forward = transform.TransformDirection(Vector3.right);
        float curSpeed = speed * Input.GetAxis("Horizontal");
        controller.SimpleMove(forward * curSpeed);
    }
}
